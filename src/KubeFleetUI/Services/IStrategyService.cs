using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IStrategyService
{
    Task<List<StagedUpdateStrategy>> ListAsync(string? ns = null);
    Task<StagedUpdateStrategy> GetAsync(string name, string? ns = null);
}
