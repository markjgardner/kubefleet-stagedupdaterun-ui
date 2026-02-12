using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class UpdateRunSpec
{
    [JsonPropertyName("placementName")]
    public string PlacementName { get; set; } = string.Empty;

    [JsonPropertyName("resourceSnapshotIndex")]
    public string? ResourceSnapshotIndex { get; set; }

    [JsonPropertyName("stagedRolloutStrategyName")]
    public string StagedUpdateStrategyName { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UpdateRunState State { get; set; } = UpdateRunState.Initialize;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UpdateRunState
{
    Initialize,
    Run,
    Stop
}
