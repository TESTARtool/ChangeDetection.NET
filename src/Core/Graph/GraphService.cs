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

public class GraphService : IGraphService
{
    private readonly IChangeDetectionHttpClient httpClient;

    public GraphService(IChangeDetectionHttpClient httpClient)
    {
        this.httpClient = httpClient;
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
        return httpClient.DocumentAsync(new OrientDbId(id.Replace('_', ':')));
    }

    public async Task<Element[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier)
    {
        var command = new OrientDbCommand("SELECT FROM (TRAVERSE inE() FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'isAbstractedBy'")
            .AddParameter("modelIdentifier", modelIdentifier.Value);

        var edges = FetchEdges(command, "isAbstractedBy");

        return await edges.ToArrayAsync();
    }

    public async Task<Element[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        List<Element> elements = new();

        if (showCompoundGraph)
        {
            elements.Add(new Element(Element.GroupNodes, new Vertex("ConcreteLayer"), "Parent"));
        }

        var command = new OrientDbCommand("SELECT FROM (TRAVERSE in() FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteState'")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var concreteNodes = await FetchNodes(command, "ConcreteState", showCompoundGraph ? "ConcreteLayer" : null, modelIdentifier).ToArrayAsync();
        elements.AddRange(concreteNodes);

        command = new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').outE('ConcreteAction') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteAction'")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var concreteActionEdges = await FetchEdges(command, "ConcreteAction").ToArrayAsync();

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

        var command = new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var nodes = await FetchNodes(command, "AbstractState", showCompoundGraph ? "AbstractLayer" : null, modelIdentifier).ToArrayAsync();
        elements.AddRange(nodes);

        command = new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var actionNodes = await FetchEdges(command, "AbstractAction").ToArrayAsync();
        elements.AddRange(actionNodes);

        command = new OrientDbCommand("SELECT FROM (TRAVERSE out() FROM  (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'BlackHole'")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var blackholeNodes = await FetchNodes(command, "BlackHole", showCompoundGraph ? "AbstractLayer" : null, modelIdentifier).ToArrayAsync();
        elements.AddRange(blackholeNodes);

        command = new OrientDbCommand("SELECT FROM UnvisitedAbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value);
        var unvisitedEdges = await FetchEdges(command, "UnvisitedAbstractAction").ToArrayAsync();
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

    private async IAsyncEnumerable<Element> FetchEdges(OrientDbCommand command, string className)
    {
        var result = await httpClient.QueryAsync<JsonElement>(command);

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

    private async IAsyncEnumerable<Element> FetchNodes(OrientDbCommand command, string className, string? parent, ModelIdentifier modelIdentifier)
    {
        var result = await httpClient.QueryAsync<JsonElement>(command);

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