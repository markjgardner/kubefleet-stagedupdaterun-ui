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

    public async Task<List<ClusterStagedUpdateRun>> ListAsync()
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.ListClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<ClusterStagedUpdateRun>>(json);
        return list?.Items ?? new List<ClusterStagedUpdateRun>();
    }

    public async Task<ClusterStagedUpdateRun> GetAsync(string name)
    {
        var client = _clientFactory.CreateClient();

        var result = await client.CustomObjects.GetClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<ClusterStagedUpdateRun>(json)!;
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

    public async Task DeleteAsync(string name)
    {
        var client = _clientFactory.CreateClient();

        await client.CustomObjects.DeleteClusterCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            name);
    }

    public async Task<ClusterStagedUpdateRun> UpdateStateAsync(string name, UpdateRunState newState)
    {
        var client = _clientFactory.CreateClient();

        var patch = new
        {
            spec = new { state = newState.ToString() }
        };

        var patchJson = JsonSerializer.Serialize(patch);
        var patchBody = new k8s.Models.V1Patch(patchJson, k8s.Models.V1Patch.PatchType.MergePatch);

        var result = await client.CustomObjects.PatchClusterCustomObjectAsync(
            patchBody,
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            KubeFleetConstants.ClusterStagedUpdateRunPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<ClusterStagedUpdateRun>(json)!;
    }
}
