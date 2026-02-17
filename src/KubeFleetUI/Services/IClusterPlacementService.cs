using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IClusterPlacementService
{
    Task<List<ClusterResourcePlacement>> ListAsync();
}
