using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class UpdateStrategySpec
{
    [JsonPropertyName("stages")]
    public List<StageConfig> Stages { get; set; } = new();
}
