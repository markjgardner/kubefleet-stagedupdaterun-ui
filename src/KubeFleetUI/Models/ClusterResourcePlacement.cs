using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ClusterResourcePlacement
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = $"{KubeFleetConstants.Group}/{KubeFleetConstants.PlacementVersion}";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "ClusterResourcePlacement";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();
}
