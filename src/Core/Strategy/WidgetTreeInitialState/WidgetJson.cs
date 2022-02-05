using System.Text.Json.Serialization;

namespace Testar.ChangeDetection.Core.Strategy.WidgetTreeInitialState;

public class WidgetJson
{
    [JsonPropertyName("in_isChildOf")]
    public string[] InIsChildOf { get; init; }

    [JsonPropertyName("out_isChildOf")]
    public string[] OutIsChildOf { get; init; }

    public string Role { get; init; }

    public string Title { get; set; }

    public Dictionary<string, string> Properties { get; } = new();
}