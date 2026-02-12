using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class ClusterUpdatingStatus
{
    [JsonPropertyName("clusterName")]
    public string ClusterName { get; set; } = string.Empty;

    [JsonPropertyName("conditions")]
    public List<Condition>? Conditions { get; set; }
}
