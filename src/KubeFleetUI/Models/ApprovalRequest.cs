using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ApprovalRequest
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "placement.kubefleet.io/v1beta1";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "ApprovalRequest";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();

    [JsonPropertyName("spec")]
    public ApprovalRequestSpec Spec { get; set; } = new();

    [JsonPropertyName("status")]
    public ApprovalRequestStatus? Status { get; set; }
}

public class ApprovalRequestSpec
{
    [JsonPropertyName("parentStageRollout")]
    public string TargetUpdateRun { get; set; } = string.Empty;

    [JsonPropertyName("targetStage")]
    public string TargetStage { get; set; } = string.Empty;
}

public class ApprovalRequestStatus
{
    [JsonPropertyName("conditions")]
    public List<Condition>? Conditions { get; set; }
}
