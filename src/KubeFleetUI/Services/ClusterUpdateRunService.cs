using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class ClusterUpdateRunService : IClusterUpdateRunService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly ILogger<ClusterUpdateRunService> _logger;

    public ClusterUpdateRunService(
        IKubernetesClientFactory clientFactory,
        ILogger<ClusterUpdateRunService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<ClusterStagedUpdateRun> CreateAsync(ClusterStagedUpdateRun run)
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.CreateClusterCustomObjectAsync(
            run,
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<ClusterStagedUpdateRun>(json)!;
    }
}
