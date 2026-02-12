using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class KubeList<T>
{
    [JsonPropertyName("apiVersion")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    [JsonPropertyName("metadata")]
    public ListMetadata? Metadata { get; set; }

    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();
}

public class ListMetadata
{
    [JsonPropertyName("resourceVersion")]
    public string? ResourceVersion { get; set; }

    [JsonPropertyName("continue")]
    public string? Continue { get; set; }
}
