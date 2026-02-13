using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class ClusterStrategyService : IClusterStrategyService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly ILogger<ClusterStrategyService> _logger;

    public ClusterStrategyService(
        IKubernetesClientFactory clientFactory,
        ILogger<ClusterStrategyService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<List<ClusterStagedUpdateStrategy>> ListAsync()
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.ListClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateStrategyPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<ClusterStagedUpdateStrategy>>(json);
        return list?.Items ?? new List<ClusterStagedUpdateStrategy>();
    }

    public async Task<ClusterStagedUpdateStrategy> GetAsync(string name)
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.GetClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateStrategyPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<ClusterStagedUpdateStrategy>(json)!;
    }
}
