namespace Testar.ChangeDetection.Core.Graph;

public class GraphElement
{
    public static readonly string GroupNodes = "nodes";

    public static readonly string GroupEdges = "edges";

    public GraphElement(string group, Document document, string typeName)
    {
        this.Group = group;
        this.Document = document;
        this.TypeName = typeName;
        Classes.Add(typeName);
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

    [JsonIgnore]
    public string TypeName { get; set; }

    [JsonIgnore]
    public bool IsInitial => Classes.Any(x => x == "isInitial");

    [JsonIgnore]
    public bool IsAbstractState => Classes.Any(x => x == "AbstractState" && Document is Vertex);

    [JsonIgnore]
    public bool IsConcreteState => Classes.Any(x => x == "ConcreteState" && Document is Vertex);

    [JsonIgnore]
    public bool IsTestSequence => Classes.Any(x => x == "TestSequence" && Document is Vertex);

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

    public Edge DocumentAsEdge()
    {
        return (Document is Edge edge)
            ? edge
            : throw new InvalidOperationException("Document is not of type Edge");
    }

    public Vertex DocumentAsVertex()
    {
        return Document is Vertex vertex
            ? vertex
            : throw new InvalidOperationException("Document is not of type Vertex");
    }

    public void AddClass(string className)
    {
        Classes.Add(className);
    }
}