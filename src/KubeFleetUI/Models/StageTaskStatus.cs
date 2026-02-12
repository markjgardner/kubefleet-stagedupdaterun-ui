using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StageTaskStatus
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StageTaskType Type { get; set; }

    [JsonPropertyName("approvalRequestName")]
    public string? ApprovalRequestName { get; set; }

    [JsonPropertyName("conditions")]
    public List<Condition>? Conditions { get; set; }
}
