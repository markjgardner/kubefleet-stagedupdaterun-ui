using Dapr.Client;

namespace KubeFleetUI.Dapr;

public class DaprSecretStore : IDaprSecretStore
{
    private readonly DaprClient _daprClient;
    private readonly string _storeName;

    public DaprSecretStore(DaprClient daprClient, IConfiguration configuration)
    {
        _daprClient = daprClient;
        _storeName = configuration["Dapr:SecretStoreName"] ?? "secretstore";
    }

    public async Task<Dictionary<string, string>> GetSecretAsync(string secretName, CancellationToken ct = default)
    {
        return await _daprClient.GetSecretAsync(_storeName, secretName, cancellationToken: ct);
    }
}
