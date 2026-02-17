using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Configuration;

namespace KubeFleetUI.Tests.Integration;

/// <summary>
/// Fixture for managing Kubernetes cluster connections in integration tests.
/// Set KUBECONFIG environment variable to point to a test cluster with KubeFleet CRDs installed.
/// </summary>
public class KubernetesClusterFixture : IDisposable
{
    private static readonly System.Text.RegularExpressions.Regex RetryAfterRegex = 
        new(@"""retryAfterSeconds""\s*:\s*(\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);

    public IKubernetes Client { get; }
    public string TestNamespace { get; }

    public KubernetesClusterFixture()
    {
        // Check if KUBECONFIG is set for integration tests
        var kubeconfigPath = Environment.GetEnvironmentVariable("KUBECONFIG");
        if (string.IsNullOrEmpty(kubeconfigPath))
        {
            throw new InvalidOperationException(
                "KUBECONFIG environment variable must be set to run integration tests. " +
                "Point it to a test cluster with KubeFleet CRDs installed.");
        }

        // Load kubeconfig
        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeconfigPath);
        Client = new Kubernetes(config);

        // Create a unique test namespace (max 63 chars for DNS-1123)
        // kubefleet-test- is 15 chars, leave room for 16-char GUID portion
        var guid = Guid.NewGuid().ToString("N")[..16]; // Take first 16 chars of GUID
        TestNamespace = $"kubefleet-test-{guid}"; // Total: 31 chars, well under 63 limit
        CreateTestNamespace().GetAwaiter().GetResult();
    }

    private async Task CreateTestNamespace()
    {
        var ns = new V1Namespace
        {
            Metadata = new V1ObjectMeta
            {
                Name = TestNamespace,
                Labels = new Dictionary<string, string>
                {
                    ["test"] = "true",
                    ["created-by"] = "integration-test"
                }
            }
        };

        try
        {
            await Client.CoreV1.CreateNamespaceAsync(ns);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create test namespace: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Executes an async operation with retry logic for transient Kubernetes API errors.
    /// Specifically handles 429 TooManyRequests errors that occur when the API server storage is initializing.
    /// </summary>
    public async Task<T> WithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 5)
    {
        int retryCount = 0;
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (HttpOperationException ex) when (ex.Response?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                if (retryCount >= maxRetries)
                {
                    throw;
                }

                var delaySeconds = CalculateRetryDelay(ex, retryCount);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                retryCount++;
            }
        }
    }

    /// <summary>
    /// Executes an async operation with retry logic, handling cases where no return value is expected.
    /// </summary>
    public async Task WithRetryAsync(Func<Task> operation, int maxRetries = 5)
    {
        int retryCount = 0;
        while (true)
        {
            try
            {
                await operation();
                return;
            }
            catch (HttpOperationException ex) when (ex.Response?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                if (retryCount >= maxRetries)
                {
                    throw;
                }

                var delaySeconds = CalculateRetryDelay(ex, retryCount);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                retryCount++;
            }
        }
    }

    /// <summary>
    /// Calculates the retry delay based on the API response or exponential backoff.
    /// </summary>
    private int CalculateRetryDelay(HttpOperationException ex, int retryCount)
    {
        // Extract retry-after seconds from response if available
        if (ex.Response?.Content != null)
        {
            var match = RetryAfterRegex.Match(ex.Response.Content);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int parsedDelay))
            {
                return parsedDelay;
            }
        }

        // Use exponential backoff: 1s, 2s, 4s, 8s, 16s (2^0, 2^1, 2^2, 2^3, 2^4)
        return 1 << retryCount;
    }

    public void Dispose()
    {
        // Clean up test namespace
        try
        {
            Client.CoreV1.DeleteNamespace(TestNamespace);
        }
        catch
        {
            // Ignore cleanup errors
        }

        Client?.Dispose();
    }
}
