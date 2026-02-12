using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public interface IUpdateRunService
{
    Task<List<StagedUpdateRun>> ListAsync(string? namespaceFilter = null);
    Task<StagedUpdateRun> GetAsync(string name, string? ns = null);
    Task<StagedUpdateRun> CreateAsync(StagedUpdateRun run);
    Task DeleteAsync(string name, string? ns = null);
    Task<StagedUpdateRun> UpdateStateAsync(string name, string? ns, UpdateRunState newState);
    IAsyncEnumerable<WatchEvent<StagedUpdateRun>> WatchAsync(string? ns = null, CancellationToken ct = default);
}
