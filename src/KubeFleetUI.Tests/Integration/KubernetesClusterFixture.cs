using k8s;
using k8s.Models;
using Microsoft.Extensions.Configuration;

namespace KubeFleetUI.Tests.Integration;

/// <summary>
/// Fixture for managing Kubernetes cluster connections in integration tests.
/// Set KUBECONFIG environment variable to point to a test cluster with KubeFleet CRDs installed.
/// </summary>
public class KubernetesClusterFixture : IDisposable
{
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
