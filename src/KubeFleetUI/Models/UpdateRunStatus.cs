using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class UpdateRunStatus
{
    [JsonPropertyName("policySnapshotIndexUsed")]
    public string? PolicySnapshotIndexUsed { get; set; }

    [JsonPropertyName("policyObservedClusterCount")]
    public int PolicyObservedClusterCount { get; set; }

    [JsonPropertyName("resourceSnapshotIndexUsed")]
    public string? ResourceSnapshotIndexUsed { get; set; }

    [JsonPropertyName("appliedStrategy")]
    public object? AppliedStrategy { get; set; }

    [JsonPropertyName("stagedUpdateStrategySnapshot")]
    public UpdateStrategySpec? UpdateStrategySnapshot { get; set; }

    [JsonPropertyName("stagesStatus")]
    public List<StageUpdatingStatus>? StagesStatus { get; set; }

    [JsonPropertyName("deletionStageStatus")]
    public StageUpdatingStatus? DeletionStageStatus { get; set; }

    [JsonPropertyName("conditions")]
    public List<Condition>? Conditions { get; set; }
}
