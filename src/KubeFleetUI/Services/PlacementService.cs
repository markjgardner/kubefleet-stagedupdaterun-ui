using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class PlacementService : IPlacementService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PlacementService> _logger;

    public PlacementService(
        IKubernetesClientFactory clientFactory,
        IConfiguration configuration,
        ILogger<PlacementService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string DefaultNamespace => _configuration["Kubernetes:DefaultNamespace"] ?? "fleet-default";

    public async Task<List<ResourcePlacement>> ListAsync(string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var result = await client.CustomObjects.ListNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.ResourcePlacementPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<ResourcePlacement>>(json);
        return list?.Items ?? new List<ResourcePlacement>();
    }
}
