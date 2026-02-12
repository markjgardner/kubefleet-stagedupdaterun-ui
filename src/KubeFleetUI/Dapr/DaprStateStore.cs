using Dapr.Client;

namespace KubeFleetUI.Dapr;

public class DaprStateStore : IDaprStateStore
{
    private readonly DaprClient _daprClient;
    private readonly string _storeName;

    public DaprStateStore(DaprClient daprClient, IConfiguration configuration)
    {
        _daprClient = daprClient;
        _storeName = configuration["Dapr:StateStoreName"] ?? "statestore";
    }

    public async Task<T?> GetStateAsync<T>(string key, CancellationToken ct = default)
    {
        return await _daprClient.GetStateAsync<T>(_storeName, key, cancellationToken: ct);
    }

    public async Task SaveStateAsync<T>(string key, T value, CancellationToken ct = default)
    {
        await _daprClient.SaveStateAsync(_storeName, key, value, cancellationToken: ct);
    }

    public async Task DeleteStateAsync(string key, CancellationToken ct = default)
    {
        await _daprClient.DeleteStateAsync(_storeName, key, cancellationToken: ct);
    }
}
