using System.Net.Http.Headers;
using System.Text.Json;

namespace Testar.ChangeDetection.Core.Graph;

public interface IGraphService
{
    Task<Element[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<Element[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<Element[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<Element[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier);

    Task<byte[]> DownloadScreenshotAsync(string id);

    string GenerateJsonString(Element[] elements);
}

public class Edge : Document
{
    public Edge(string id, string sourceId, string targetId) : base(id, sourceId, targetId)
    {
    }
}

public class Vertex : Document
{
    public Vertex(string id) : base(id)
    {
    }
}

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

public class GraphService : IGraphService
{
    private readonly IOrientDbCommandExecuter orientDbCommand;

    public GraphService(IOrientDbCommandExecuter orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public string GenerateJsonString(Element[] elements)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(elements, options);
    }

    public Task<byte[]> DownloadScreenshotAsync(string id)
    {
        return orientDbCommand.ExecuteDocumentAsync(new OrientDbId(id.Replace('_', ':')));
    }

    public async Task<Element[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier)
    {
        var sql = $"SELECT FROM (TRAVERSE inE() FROM (SELECT FROM AbstractState WHERE modelIdentifier = '{modelIdentifier.Value}')) WHERE @class = 'isAbstractedBy'";
        var edges = FetchEdges(sql, "isAbstractedBy");

        return await edges.ToArrayAsync();
    }

    public async Task<Element[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        List<Element> elements = new();

        if (showCompoundGraph)
        {
            elements.Add(new Element(Element.GroupNodes, new Vertex("ConcreteLayer"), "Parent"));
        }

        var concreteSql = $"SELECT FROM (TRAVERSE in() FROM (SELECT FROM AbstractState WHERE modelIdentifier = '{modelIdentifier.Value}')) WHERE @class = 'ConcreteState'";
        var concreteNodes = await FetchNodes(concreteSql, "ConcreteState", showCompoundGraph ? "ConcreteLayer" : null, modelIdentifier)
            .ToArrayAsync();

        elements.AddRange(concreteNodes);

        var concreteActionSql = $"SELECT FROM (TRAVERSE in('isAbstractedBy').outE('ConcreteAction') FROM (SELECT FROM AbstractState WHERE modelIdentifier = '{modelIdentifier.Value}')) WHERE @class = 'ConcreteAction'";
        var concreteActionEdges = await FetchEdges(concreteActionSql, "ConcreteAction")
            .ToArrayAsync();

        elements.AddRange(concreteActionEdges);

        return elements.ToArray();
    }

    public async Task<Element[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        List<Element> elements = new();

        if (showCompoundGraph)
        {
            elements.Add(new Element(Element.GroupNodes, new Vertex("AbstractLayer"), "Parent"));
        }

        var nodesCommand = new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value);

        var nodesSql = $"SELECT FROM AbstractState WHERE modelIdentifier = '{modelIdentifier.Value}'";
        var nodes = await FetchNodes(nodesSql, "AbstractState", showCompoundGraph ? "AbstractLayer" : null, modelIdentifier)
            .ToArrayAsync();

        elements.AddRange(nodes);

        var actionSql = $"SELECT FROM AbstractAction WHERE modelIdentifier = '{modelIdentifier.Value}'";
        var actionNodes = await FetchEdges(actionSql, "AbstractAction")
            .ToArrayAsync();

        elements.AddRange(actionNodes);

        var blackholeSql = $"SELECT FROM (TRAVERSE out() FROM  (SELECT FROM AbstractState WHERE modelIdentifier = '{modelIdentifier.Value}')) WHERE @class = 'BlackHole'";
        var blackholeNodes = await FetchNodes(blackholeSql, "BlackHole", showCompoundGraph ? "AbstractLayer" : null, modelIdentifier)
            .ToArrayAsync();

        elements.AddRange(blackholeNodes);

        var unvisitedSql = $"SELECT FROM UnvisitedAbstractAction WHERE modelIdentifier = '{modelIdentifier.Value}'";
        var unvisitedEdges = await FetchEdges(unvisitedSql, "UnvisitedAbstractAction")
            .ToArrayAsync();

        elements.AddRange(unvisitedEdges);

        return elements.ToArray();
    }

    public async Task<Element[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        var abstractLayer = await FetchAbstractLayerAsync(modelIdentifier, showCompoundGraph);
        var concreteLayer = await FetchConcreteLayerAsync(modelIdentifier, showCompoundGraph);
        var connections = await FetchAbstractConcreteConnectors(modelIdentifier);

        var elements = new List<Element>();
        elements.AddRange(abstractLayer);
        elements.AddRange(concreteLayer);
        elements.AddRange(connections);

        return elements.ToArray();
    }

    private async IAsyncEnumerable<Element> FetchEdges(string sql, string className)
    {
        var result = await orientDbCommand.ExecuteQueryAsync<JsonElement>(sql);
        foreach (var item in result)
        {
            var id = new OrientDbId(item.GetProperty("@rid").ToString());
            var sourceId = new OrientDbId(item.GetProperty("out").ToString());
            var targetId = new OrientDbId(item.GetProperty("in").ToString());
            var jsonEdge = new Edge("e" + FormatId(id), "n" + FormatId(sourceId), "n" + FormatId(targetId));

            foreach (var property in item.EnumerateObject())
            {
                if (!property.Name.Contains("in") && !property.Name.Contains("out") && !property.Name.StartsWith("@"))
                {
                    jsonEdge.AddProperty(property.Name, property.Value.ToString().Replace("\"", ""));
                }
            }

            yield return new Element(Element.GroupEdges, jsonEdge, className);
        }
    }

    private async IAsyncEnumerable<Element> FetchNodes(string sql, string className, string? parent, ModelIdentifier modelIdentifier)
    {
        var result = await orientDbCommand.ExecuteQueryAsync<JsonElement>(sql);
        foreach (var item in result)
        {
            var id = new OrientDbId(item.GetProperty("@rid").ToString());

            var jsonVertex = new Vertex("n" + FormatId(id));

            foreach (var property in item.EnumerateObject())
            {
                if (property.Name.Contains("in_") || property.Name.Contains("out_") || property.Name.StartsWith("@"))
                {
                }
                else if (property.Name == "screenshot")
                {
                    var orientDb = new OrientDbId(property.Value.ToString());
                    var formattedId = FormatId(orientDb);

                    jsonVertex.AddProperty("screenshot", formattedId);
                }
                else
                {
                    jsonVertex.AddProperty(property.Name, property.Value.ToString().Replace("\"", ""));
                }
            }

            if (parent is not null)
            {
                jsonVertex.AddProperty("parent", parent);
            }

            var element = new Element(Element.GroupNodes, jsonVertex, className);
            if (item.TryGetProperty("isInitial", out var isInitialElement))
            {
                if (isInitialElement.GetBoolean())
                {
                    element.AddClass("isInitial");
                }
            }

            yield return element;
        }
    }

    // this helper method formats the @RID property into something that can be used in a web frontend
    private string FormatId(OrientDbId id)
    {
        if (id.Id.IndexOf("#") != 0) return id.Id; // not an orientdb id
        return id.Id
            .Replace("#", "")
            .Replace(":", "_");
    }
}

public class ChangeDetectionHttpClient
{
    private readonly JsonSerializerOptions jsonOptions;

    public ChangeDetectionHttpClient(HttpClient client)
    {
        this.HttpClient = client;

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public HttpClient HttpClient { get; }

    public void SetAuthenticationToken(string authentication)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authentication);
    }

    public void SetBaseAddress(Uri baseAddress)
    {
        HttpClient.BaseAddress = baseAddress;
    }

    public async Task<T> QueryAsync<T>(OrientDbCommand command)
    {
        var url = $"/query";

        var json = JsonSerializer.Serialize(command);
        using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json),
        };

        var response = await HttpClient.SendAsync(httpContent);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return orientDbResult;
    }
}