namespace Testar.ChangeDetection.Core.Graph;

public class Element
{
    public const string GroupNodes = "nodes";

    public const string GroupEdges = "edges";

    public Element(string group, Document document)
    {
        this.Group = group;
        this.Document = document;
    }

    public Element(string group, Document document, string className)
    {
        this.Group = group;
        this.Document = document;
        Classes.Add(className);
    }

    [JsonInclude]
    [JsonPropertyName("group")]
    public string Group { get; set; }

    [JsonInclude]
    [JsonPropertyName("data")]
    public Document Document { get; set; }

    [JsonInclude]
    [JsonPropertyName("classes")]
    public List<string> Classes { get; set; } = new();

    public void AddClass(String className)
    {
        Classes.Add(className);
    }
}
