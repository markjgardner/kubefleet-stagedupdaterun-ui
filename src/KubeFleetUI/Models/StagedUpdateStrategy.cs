using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StagedUpdateStrategy
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "placement.kubernetes-fleet.io/v1beta1";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "StagedUpdateStrategy";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();

    [JsonPropertyName("spec")]
    public UpdateStrategySpec Spec { get; set; } = new();
}
