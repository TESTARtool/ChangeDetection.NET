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

    [JsonIgnore]
    public bool IsHandeld { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public PropertyValue this[string name]
    {
        get
        {
            return new PropertyValue(Property(name));
        }
        set
        {
            AddProperty(name, value.Value);
        }
    }

    public void AddProperty(string key, string value)
    {
        if (this.Properties.ContainsKey(key))
        {
            this.Properties[key] = value;
        }
        else
        {
            Properties.Add(key, value);
        }
    }

    public string Property(string key)
    {
        return Properties.TryGetValue(key, out var value)
            ? value?.ToString() ?? string.Empty
            : string.Empty;
    }
}