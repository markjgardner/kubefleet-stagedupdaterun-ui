using Azure.Identity;
using k8s;

namespace KubeFleetUI.Services;

public class KubernetesClientFactory : IKubernetesClientFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KubernetesClientFactory> _logger;

    public KubernetesClientFactory(IConfiguration configuration, ILogger<KubernetesClientFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IKubernetes CreateClient()
    {
        var apiServerUrl = _configuration["Kubernetes:ApiServerUrl"];

        if (string.IsNullOrEmpty(apiServerUrl))
        {
            // Try in-cluster config
            _logger.LogInformation("No API server URL configured, attempting in-cluster config");
            var config = KubernetesClientConfiguration.InClusterConfig();
            return new Kubernetes(config);
        }

        _logger.LogInformation("Creating Kubernetes client for {ApiServer}", apiServerUrl);
        var clientConfig = new KubernetesClientConfiguration
        {
            Host = apiServerUrl,
            AccessToken = GetAccessToken()
        };

        return new Kubernetes(clientConfig);
    }

    private string GetAccessToken()
    {
        try
        {
            var credential = new DefaultAzureCredential();
            // Azure Kubernetes Service AAD server application ID
            var scope = _configuration["Kubernetes:AadScope"] ?? "6dae42f8-4368-4678-94ff-3960e28e3630/.default";
            var tokenRequestContext = new Azure.Core.TokenRequestContext(new[] { scope });
            var token = credential.GetToken(tokenRequestContext);
            return token.Token;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to obtain Azure AD token, using empty token");
            return string.Empty;
        }
    }
}
