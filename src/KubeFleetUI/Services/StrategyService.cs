using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class StrategyService : IStrategyService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StrategyService> _logger;

    public StrategyService(
        IKubernetesClientFactory clientFactory,
        IConfiguration configuration,
        ILogger<StrategyService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string DefaultNamespace => _configuration["Kubernetes:DefaultNamespace"] ?? "fleet-default";

    public async Task<List<StagedUpdateStrategy>> ListAsync(string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var result = await client.CustomObjects.ListNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateStrategyPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<StagedUpdateStrategy>>(json);
        return list?.Items ?? new List<StagedUpdateStrategy>();
    }

    public async Task<StagedUpdateStrategy> GetAsync(string name, string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var result = await client.CustomObjects.GetNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateStrategyPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<StagedUpdateStrategy>(json)!;
    }
}
