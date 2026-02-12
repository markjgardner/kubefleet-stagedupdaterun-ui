using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StageUpdatingStatus
{
    [JsonPropertyName("stageName")]
    public string StageName { get; set; } = string.Empty;

    [JsonPropertyName("clusters")]
    public List<ClusterUpdatingStatus> Clusters { get; set; } = new();

    [JsonPropertyName("afterStageTaskStatus")]
    public List<StageTaskStatus>? AfterStageTaskStatus { get; set; }

    [JsonPropertyName("beforeStageTaskStatus")]
    public List<StageTaskStatus>? BeforeStageTaskStatus { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    [JsonPropertyName("conditions")]
    public List<Condition>? Conditions { get; set; }
}
