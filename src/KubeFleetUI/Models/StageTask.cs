using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StageTask
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StageTaskType Type { get; set; }

    [JsonPropertyName("waitTime")]
    public string? WaitTime { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StageTaskType
{
    TimedWait,
    Approval
}
