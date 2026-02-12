namespace KubeFleetUI.Dapr;

public interface IDaprSecretStore
{
    Task<Dictionary<string, string>> GetSecretAsync(string secretName, CancellationToken ct = default);
}
