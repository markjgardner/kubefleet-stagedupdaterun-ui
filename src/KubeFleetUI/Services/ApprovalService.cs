using System.Text.Json;
using k8s;
using KubeFleetUI.Models;

namespace KubeFleetUI.Services;

public class ApprovalService : IApprovalService
{
    private readonly IKubernetesClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(
        IKubernetesClientFactory clientFactory,
        IConfiguration configuration,
        ILogger<ApprovalService> logger)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string DefaultNamespace => _configuration["Kubernetes:DefaultNamespace"] ?? "fleet-default";

    public async Task<List<ApprovalRequest>> ListForRunAsync(string updateRunName, string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var labelSelector = $"{KubeFleetConstants.LabelTargetUpdateRun}={updateRunName},{KubeFleetConstants.LabelIsLatestApproval}=true";

        var result = await client.CustomObjects.ListNamespacedCustomObjectAsync(
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.ApprovalRequestPlural,
            labelSelector: labelSelector);

        var json = JsonSerializer.Serialize(result);
        var list = JsonSerializer.Deserialize<KubeList<ApprovalRequest>>(json);
        return list?.Items ?? new List<ApprovalRequest>();
    }

    public async Task ApproveAsync(string approvalRequestName, string? ns = null)
    {
        var client = _clientFactory.CreateClient();
        var namespaceName = ns ?? DefaultNamespace;

        var statusPatch = new
        {
            status = new
            {
                conditions = new[]
                {
                    new
                    {
                        type = "Approved",
                        status = "True",
                        reason = "ManualApproval",
                        message = "Approved via KubeFleet UI",
                        lastTransitionTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                }
            }
        };

        var patchJson = JsonSerializer.Serialize(statusPatch);
        var patchBody = new k8s.Models.V1Patch(patchJson, k8s.Models.V1Patch.PatchType.MergePatch);

        await client.CustomObjects.PatchNamespacedCustomObjectStatusAsync(
            patchBody,
            KubeFleetConstants.Group,
            KubeFleetConstants.Version,
            namespaceName,
            KubeFleetConstants.ApprovalRequestPlural,
            approvalRequestName);
    }
}
