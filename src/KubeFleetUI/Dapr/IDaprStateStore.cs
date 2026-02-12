namespace KubeFleetUI.Dapr;

public interface IDaprStateStore
{
    Task<T?> GetStateAsync<T>(string key, CancellationToken ct = default);
    Task SaveStateAsync<T>(string key, T value, CancellationToken ct = default);
    Task DeleteStateAsync(string key, CancellationToken ct = default);
}
