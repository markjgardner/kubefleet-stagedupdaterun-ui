using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IClusterUpdateRunService
{
    Task<List<ClusterStagedUpdateRun>> ListAsync();
    Task<ClusterStagedUpdateRun> GetAsync(string name);
    Task<ClusterStagedUpdateRun> CreateAsync(ClusterStagedUpdateRun run);
    Task DeleteAsync(string name);
    Task<ClusterStagedUpdateRun> UpdateStateAsync(string name, UpdateRunState newState);
}
