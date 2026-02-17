using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IPlacementService
{
    Task<List<ResourcePlacement>> ListAsync(string? ns = null);
}
