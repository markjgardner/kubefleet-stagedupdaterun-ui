using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ClusterStagedUpdateRun
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "placement.kubernetes-fleet.io/v1beta1";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "ClusterStagedUpdateRun";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();

    [JsonPropertyName("spec")]
    public UpdateRunSpec Spec { get; set; } = new();

    [JsonPropertyName("status")]
    public UpdateRunStatus? Status { get; set; }
}
