using System.Text.Json.Serialization;

namespace KubeFleetUI.Models;

public class WatchEvent<T>
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public T Object { get; set; } = default!;
}

public static class WatchEventTypes
{
    public const string Added = "ADDED";
    public const string Modified = "MODIFIED";
    public const string Deleted = "DELETED";
    public const string Error = "ERROR";
}
