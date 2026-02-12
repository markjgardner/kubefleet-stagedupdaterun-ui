using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ResourceMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("namespace")]
    public string? Namespace { get; set; }

    [JsonPropertyName("uid")]
    public string? Uid { get; set; }

    [JsonPropertyName("resourceVersion")]
    public string? ResourceVersion { get; set; }

    [JsonPropertyName("creationTimestamp")]
    public DateTime? CreationTimestamp { get; set; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string>? Labels { get; set; }

    [JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; set; }
}
