namespace Testar.ChangeDetection.Core.Graph;

public class GraphElement
{
    public const string GroupNodes = "nodes";

    public const string GroupEdges = "edges";

    public GraphElement(string group, Document document, string typeName)
    {
        this.Group = group;
        this.Document = document;
        this.TypeName = typeName;
        Classes.Add(typeName);
    }

    [JsonIgnore]
    public Edge DocumentAsEdge => (Edge)Document;

    [JsonInclude]
    [JsonPropertyName("group")]
    public string Group { get; set; }

    [JsonInclude]
    [JsonPropertyName("data")]
    public Document Document { get; set; }

    [JsonInclude]
    [JsonPropertyName("classes")]
    public List<string> Classes { get; set; } = new();

    [JsonIgnore]
    public string TypeName { get; set; }

    [JsonIgnore]
    public Vertex DocumentAsVertex => (Vertex)Document;

    [JsonIgnore]
    public bool IsInitial => Classes.Any(x => x == "isInitial");

    [JsonIgnore]
    public bool IsAbstractState => Classes.Any(x => x == "AbstractState" && Document is Vertex);

    [JsonIgnore]
    public bool IsConcreteState => Classes.Any(x => x == "ConcreteState" && Document is Vertex);

    [JsonIgnore]
    public bool IsAbstractAction => Classes.Any(x => x == "AbstractAction" && Document is Edge);

    [JsonIgnore]
    public bool IsConcreteAction => Classes.Any(x => x == "ConcreteAction" && Document is Edge);

    [JsonIgnore]
    public bool IsHandeld
    {
        get => Document.IsHandeld;
        set => Document.IsHandeld = value;
    }

    public PropertyValue this[string name]
    {
        get
        {
            return Document[name];
        }
        set
        {
            Document.AddProperty(name, value.Value);
        }
    }

    public void AddClass(string className)
    {
        Classes.Add(className);
    }
}