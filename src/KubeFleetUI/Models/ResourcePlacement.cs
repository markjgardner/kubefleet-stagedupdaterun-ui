using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ResourcePlacement
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = $"{KubeFleetConstants.Group}/{KubeFleetConstants.Version}";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "ResourcePlacement";

    [JsonPropertyName("metadata")]
    public ResourceMetadata Metadata { get; set; } = new();
}
