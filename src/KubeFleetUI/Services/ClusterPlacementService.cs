using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class ClusterPlacementService : IClusterPlacementService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly ILogger<ClusterPlacementService> _logger;

    public ClusterPlacementService(
        IKubernetesClientFactory clientFactory,
        ILogger<ClusterPlacementService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<List<ClusterResourcePlacement>> ListAsync()
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.ListClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.PlacementVersion,
            KubeFleetConstants.ClusterResourcePlacementPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<ClusterResourcePlacement>>(json);
        return list?.Items ?? new List<ClusterResourcePlacement>();
    }
}
