using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IClusterStrategyService
{
    Task<List<ClusterStagedUpdateStrategy>> ListAsync();
    Task<ClusterStagedUpdateStrategy> GetAsync(string name);
}
