using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class StageConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("labelSelector")]
    public LabelSelector? LabelSelector { get; set; }

    [JsonPropertyName("sortingLabelKey")]
    public string? SortingLabelKey { get; set; }

    [JsonPropertyName("maxConcurrency")]
    public object? MaxConcurrency { get; set; }

    [JsonPropertyName("afterStageTasks")]
    public List<StageTask>? AfterStageTasks { get; set; }

    [JsonPropertyName("beforeStageTasks")]
    public List<StageTask>? BeforeStageTasks { get; set; }
}

public class LabelSelector
{
    [JsonPropertyName("matchLabels")]
    public Dictionary<string, string>? MatchLabels { get; set; }

    [JsonPropertyName("matchExpressions")]
    public List<LabelSelectorRequirement>? MatchExpressions { get; set; }
}

public class LabelSelectorRequirement
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;

    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }
}
