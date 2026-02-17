using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IClusterUpdateRunService
{
    Task<ClusterStagedUpdateRun> CreateAsync(ClusterStagedUpdateRun run);
}
