namespace Testar.ChangeDetection.Core.Graph;

public abstract class Document
{
    public Document(string id)
    {
        this.Id = id;
    }

    public Document(string id, string sourceId, string targetId)
    {
        this.Id = id;
        this.SourceId = sourceId;
        this.TargetId = targetId;
    }

    [JsonInclude]
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonInclude]
    [JsonPropertyName("source")]
    public string? SourceId { get; set; }

    [JsonInclude]
    [JsonPropertyName("target")]
    public string? TargetId { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public void AddProperty(string key, string value)
    {
        Properties.Add(key, value);
    }
}