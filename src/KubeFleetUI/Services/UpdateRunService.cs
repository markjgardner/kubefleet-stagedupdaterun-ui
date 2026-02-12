using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class UpdateRunService : IUpdateRunService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateRunService> _logger;

    public UpdateRunService(
        IKubernetesClientFactory clientFactory,
        IConfiguration configuration,
        ILogger<UpdateRunService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string DefaultNamespace => _configuration["Kubernetes:DefaultNamespace"] ?? "fleet-default";

    public async Task<List<StagedUpdateRun>> ListAsync(string? namespaceFilter = null)
    {
        var client = _clientFactory.CreateClient();
        var ns = namespaceFilter ?? DefaultNamespace;

        var result = await client.CustomObjects.ListNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            ns,
            KubeFleetConstants.StagedUpdateRunPlural);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<StagedUpdateRun>>(json);
        return list?.Items ?? new List<StagedUpdateRun>();
    }

    public async Task<StagedUpdateRun> GetAsync(string name, string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var result = await client.CustomObjects.GetNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateRunPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<StagedUpdateRun>(json)!;
    }

    public async Task<StagedUpdateRun> CreateAsync(StagedUpdateRun run)
    {
        var client = _clientFactory.CreateClient();
        var ns = run.Metadata.Namespace ?? DefaultNamespace;
        run.Metadata.Namespace = ns;

        var result = await client.CustomObjects.CreateNamespacedCustomObjectAsync(
            run,
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            ns,
            KubeFleetConstants.StagedUpdateRunPlural);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<StagedUpdateRun>(json)!;
    }

    public async Task DeleteAsync(string name, string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        await client.CustomObjects.DeleteNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateRunPlural,
            name);
    }

    public async Task<StagedUpdateRun> UpdateStateAsync(string name, string? ns, UpdateRunState newState)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var patch = new
        {
            spec = new { state = newState.ToString() }
        };

        var patchJson = JsonSerializer.Serialize(patch);
        var patchBody = new k8s.Models.V1Patch(patchJson, k8s.Models.V1Patch.PatchType.MergePatch);

        var result = await client.CustomObjects.PatchNamespacedCustomObjectAsync(
            patchBody,
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateRunPlural,
            name);

        var json = JsonSerializer.Serialize(result);
        return JsonSerializer.Deserialize<StagedUpdateRun>(json)!;
    }

    public async IAsyncEnumerable<WatchEvent<StagedUpdateRun>> WatchAsync(
        string? ns = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var response = await client.CustomObjects.ListNamespacedCustomObjectWithHttpMessagesAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.StagedUpdateRunPlural,
            watch: true,
            cancellationToken: ct);

        var stream = response.Body as System.IO.Stream;
        if (stream == null) yield break;

        using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line == null) break;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var watchEvent = JsonSerializer.Deserialize<WatchEvent<StagedUpdateRun>>(line);
            if (watchEvent != null)
            {
                yield return watchEvent;
            }
        }
    }
}
