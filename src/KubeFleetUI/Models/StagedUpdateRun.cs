using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StagedUpdateRun
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "placement.kubefleet.io/v1beta1";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "StagedUpdateRun";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();

    [JsonPropertyName("spec")]
    public UpdateRunSpec Spec { get; set; } = new();

    [JsonPropertyName("status")]
    public UpdateRunStatus? Status { get; set; }
}
