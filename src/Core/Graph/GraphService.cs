﻿using System.Text.Json;

namespace Testar.ChangeDetection.Core.Graph;

public interface IGraphService
{
    Task<GraphElement[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier);

    Task<GraphElement[]> FetchSequenceLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchDiffGraph(ModelIdentifier modelIdentifier1, ModelIdentifier modelIdentifier2);

    Task<byte[]> DownloadScreenshotAsync(string id);

    string GenerateJsonString(GraphElement[] elements);
}

public class GraphService : IGraphService
{
    private readonly IChangeDetectionHttpClient httpClient;

    public GraphService(IChangeDetectionHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public string GenerateJsonString(GraphElement[] elements)
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

    public async Task<GraphElement[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier)
    {
        return await new OrientDbCommand("SELECT FROM (TRAVERSE inE() FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'isAbstractedBy'")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "isAbstractedBy"))
            .ToArrayAsync();
    }

    public async Task<GraphElement[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        List<GraphElement> elements = new();

        if (showCompoundGraph)
        {
            elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex("ConcreteLayer"), "Parent"));
        }

        var result1 = new OrientDbCommand("SELECT FROM (TRAVERSE in() FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteState'")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "ConcreteState", showCompoundGraph ? "ConcreteLayer" : null))
            .ToArrayAsync();

        var result2 = new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').outE('ConcreteAction') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteAction'")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "ConcreteAction"))
            .ToArrayAsync();

        elements.AddRange(await result1);
        elements.AddRange(await result2);

        return elements.ToArray();
    }

    public async Task<GraphElement[]> FetchDiffGraph(ModelIdentifier modelIdentifier1, ModelIdentifier modelIdentifier2)
    {
        var result1 = await new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier1.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "AbstractState", modelIdentifier1.Value, modelIdentifier1.Value))
            .ToArrayAsync();

        var result2 = await new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier2.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "AbstractState", modelIdentifier2.Value, modelIdentifier2.Value))
            .ToArrayAsync();

        var result2Docs = result2.Select(x => x.Document);
        foreach (var item in result1)
        {
            if (!result2Docs.Any(x => x.Id == item.Document.Id))
            {
                item.AddClass("Removed");
            }
        }

        var result1Docs = result1.Select(x => x.Document);
        foreach (var item in result2)
        {
            if (!result1Docs.Any(x => x.Id == item.Document.Id))
            {
                item.AddClass("Added");
            }
        }

        var result3 = new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier1.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "AbstractAction"))
            .ToArrayAsync();

        var result4 = new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier2.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "AbstractAction"))
            .ToArrayAsync();

        var elements = new List<GraphElement>
        {
            new GraphElement(GraphElement.GroupNodes, new Vertex(modelIdentifier1.Value), "Parent"),
            new GraphElement(GraphElement.GroupNodes, new Vertex(modelIdentifier2.Value), "Parent")
        };


        elements.AddRange(result1);
        elements.AddRange(result2);
        elements.AddRange(await result3);
        elements.AddRange(await result4);

        return elements.ToArray();

        // exist in result1 but missing in result2 -> removed
        // missing in result1 but exist in result1 -> added
        // only for initial state -> if Id is different -> changed
    }

    public async Task<GraphElement[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        List<GraphElement> elements = new();

        if (showCompoundGraph)
        {
            elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex("AbstractLayer"), "Parent"));
        }

        var result1 = new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "AbstractState", showCompoundGraph ? "AbstractLayer" : null))
            .ToArrayAsync();

        var result2 = new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "AbstractAction"))
            .ToArrayAsync();

        var result3 = new OrientDbCommand("SELECT FROM (TRAVERSE out() FROM  (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'BlackHole'")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "BlackHole", showCompoundGraph ? "AbstractLayer" : null))
            .ToArrayAsync();

        var result4 = new OrientDbCommand("SELECT FROM UnvisitedAbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "UnvisitedAbstractAction"))
            .ToArrayAsync();

        elements.AddRange(await result1);
        elements.AddRange(await result2);
        elements.AddRange(await result3);
        elements.AddRange(await result4);

        return elements.ToArray();
    }

    public async Task<GraphElement[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        var abstractLayer = await FetchAbstractLayerAsync(modelIdentifier, showCompoundGraph);
        var concreteLayer = await FetchConcreteLayerAsync(modelIdentifier, showCompoundGraph);
        var sequenceLayer = await FetchSequenceLayerAsync(modelIdentifier, showCompoundGraph);
        var connections = await FetchAbstractConcreteConnectors(modelIdentifier);
        var sequenceConnections = await FetchConcreteSequenceConnectors(modelIdentifier);

        var elements = new List<GraphElement>();
        elements.AddRange(abstractLayer);
        elements.AddRange(concreteLayer);
        elements.AddRange(sequenceLayer);
        elements.AddRange(connections);
        elements.AddRange(sequenceConnections);

        return elements.ToArray();
    }

    public async Task<GraphElement[]> FetchSequenceLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph)
    {
        var elements = new List<GraphElement>();

        if (showCompoundGraph)
        {
            elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex("SequenceLayer"), "Parent"));
        }

        var result1 = new OrientDbCommand("SELECT FROM TestSequence WHERE modelIdentifier = :identifier")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "TestSequence", showCompoundGraph ? "SequenceLayer" : null))
            .ToArrayAsync();

        var result2 = new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').in('Accessed') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :identifier)) WHERE @class = 'SequenceNode'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsNode(x, "SequenceNode", showCompoundGraph ? "SequenceLayer" : null))
            .ToArrayAsync();

        var result3 = new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').in('Accessed').outE('SequenceStep') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :identifier)) WHERE @class = 'SequenceStep'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "SequenceStep"))
            .ToArrayAsync();

        var result4 = new OrientDbCommand("SELECT FROM (TRAVERSE outE('FirstNode') FROM (SELECT FROM TestSequence WHERE modelIdentifier = :identifier)) WHERE @class = 'FirstNode'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "FirstNode"))
            .ToArrayAsync();

        elements.AddRange(await result1);
        elements.AddRange(await result2);
        elements.AddRange(await result3);
        elements.AddRange(await result4);

        return elements.ToArray();
    }

    private GraphElement AsEdge(JsonElement jsonElement, string className, params string[] extraClasses)
    {
        var id = new OrientDbId(jsonElement.GetProperty("@rid").ToString());
        var sourceId = new OrientDbId(jsonElement.GetProperty("out").ToString());
        var targetId = new OrientDbId(jsonElement.GetProperty("in").ToString());
        var jsonEdge = new Edge("e" + FormatId(id), "n" + FormatId(sourceId), "n" + FormatId(targetId));

        foreach (var property in jsonElement.EnumerateObject())
        {
            if (!property.Name.Contains("in") && !property.Name.Contains("out") && !property.Name.StartsWith("@"))
            {
                jsonEdge.AddProperty(property.Name, property.Value.ToString().Replace("\"", ""));
            }
        }

        var element = new GraphElement(GraphElement.GroupEdges, jsonEdge, className);

        foreach (var extraClass in extraClasses)
        {
            element.AddClass(extraClass);
        }

        return element;
    }

    private GraphElement AsNode(JsonElement jsonElement, string className, string? parent, params string[] extraClasses)
    {
        var id = new OrientDbId(jsonElement.GetProperty("@rid").ToString());

        var jsonVertex = new Vertex("n" + FormatId(id));

        foreach (var property in jsonElement.EnumerateObject())
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

        var element = new GraphElement(GraphElement.GroupNodes, jsonVertex, className);
        if (jsonElement.TryGetProperty("isInitial", out var isInitialElement))
        {
            if (isInitialElement.GetBoolean())
            {
                element.AddClass("isInitial");
            }
        }

        foreach (var extraClass in extraClasses)
        {
            element.AddClass(extraClass);
        }

        return element;
    }

    private async Task<GraphElement[]> FetchConcreteSequenceConnectors(ModelIdentifier modelIdentifier)
    {
        return await new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').inE('Accessed') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :identifier)) WHERE @class = 'Accessed'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "Accessed"))
            .ToArrayAsync();
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