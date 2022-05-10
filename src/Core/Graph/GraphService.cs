﻿using System.Text.Json;
using Testar.ChangeDetection.Core.Settings;

namespace Testar.ChangeDetection.Core.Graph;

public interface IGraphService
{
    Task<GraphElement[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier);

    Task<GraphElement[]> FetchSequenceLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchDiffGraph(ModelIdentifier modelIdentifier1, ModelIdentifier modelIdentifier2);

    Task<GraphElement[]> FetchConcreteSequenceConnectors(ModelIdentifier modelIdentifier);

    Task<string> DownloadScreenshotAsync(string id);

    string GenerateJsonString(GraphElement[] elements);
}

public class GraphService : IGraphService
{
    private readonly IChangeDetectionHttpClient httpClient;
    private readonly AbstractStateLabelSetting abstractStateLabelSetting;
    private readonly TestSequenceLabelSetting testSequenceLabelSetting;
    private readonly SequenceNodeLabelSetting sequenceNodeLabelSetting;
    private readonly ConcreteStateLabelSetting concreteStateLabelSetting;
    private readonly ShowPrefixLabelSettings showPrefixLabelSetting;

    public GraphService(IChangeDetectionHttpClient httpClient,
        AbstractStateLabelSetting abstractStateLabelSetting,
        TestSequenceLabelSetting testSequenceLabelSetting,
        SequenceNodeLabelSetting sequenceNodeLabelSetting,
        ConcreteStateLabelSetting concreteStateLabelSetting,
        ShowPrefixLabelSettings showPrefixLabelSetting)
    {
        this.httpClient = httpClient;
        this.abstractStateLabelSetting = abstractStateLabelSetting;
        this.testSequenceLabelSetting = testSequenceLabelSetting;
        this.sequenceNodeLabelSetting = sequenceNodeLabelSetting;
        this.concreteStateLabelSetting = concreteStateLabelSetting;
        this.showPrefixLabelSetting = showPrefixLabelSetting;
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

    public async Task<string> DownloadScreenshotAsync(string id)
    {
        var screenshot = await httpClient.DocumentAsBase64Async(new OrientDbId(id.Replace('_', ':')));

        return screenshot ?? string.Empty;
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
            .Select(x => AsVertex(x, "ConcreteState", showCompoundGraph ? "ConcreteLayer" : null))
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
            .Select(x => AsVertex(x, "AbstractState", modelIdentifier1.Value, modelIdentifier1.Value))
            .ToArrayAsync();

        var result2 = await new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier2.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsVertex(x, "AbstractState", modelIdentifier2.Value, modelIdentifier2.Value))
            .ToArrayAsync();

        var result2Docs = result2.Select(x => (Vertex)x.Document);
        foreach (var item in result1)
        {
            if (!result2Docs.Any(x => x.StateId == ((Vertex)item.Document).StateId))
            {
                item.AddClass("Removed");
            }
        }

        var result1Docs = result1.Select(x => (Vertex)x.Document);
        foreach (var item in result2)
        {
            if (!result1Docs.Any(x => x.StateId == ((Vertex)item.Document).StateId))
            {
                item.AddClass("Added");
            }
        }

        var elements = new List<GraphElement>();
        elements.AddRange(result1);
        elements.AddRange(result2);

        var stateId_Ids = new Dictionary<string, string>();
        var switchIdsFromLeftToRight = new Dictionary<string, string>();
        var unchangedStateIds = new List<string>();
        var unchangedIds = new List<string>();

        foreach (var item in elements)
        {
            if (!item.Classes.Any(x => x == "Added" || x == "Removed"))
            {
                item.AddClass("Unchanged");
                var stateId = ((Vertex)item.Document).StateId;
                if (stateId is not null)
                {
                    unchangedIds.Add(item.Document.Id);

                    if (unchangedStateIds.Contains(stateId))
                    {
                        // stateId already exist, switch ids
                        // lookup new Id for state
                        var newId = stateId_Ids[stateId];
                        switchIdsFromLeftToRight.Add(item.Document.Id, newId);

                        // since this item should not have any edges now, remove from graph
                        item.AddClass("Remove");
                    }
                    else
                    {
                        unchangedStateIds.Add(stateId);
                        stateId_Ids.Add(stateId, item.Document.Id);
                    }
                }
            }
        }

        elements.RemoveAll(x => x.Classes.Contains("Remove"));

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

        var edges = new List<GraphElement>();
        edges.AddRange(await result3);
        edges.AddRange(await result4);

        foreach (var item in edges)
        {
            var edge = (Edge)item.Document;
            if (edge.SourceId is not null && edge.TargetId is not null)
            {
                if (unchangedIds.Contains(edge.SourceId) && switchIdsFromLeftToRight.ContainsKey(edge.SourceId))
                {
                    // part of the unchange state, Replace ids
                    edge.SourceId = switchIdsFromLeftToRight[edge.SourceId];
                }
                if (unchangedIds.Contains(edge.TargetId) && switchIdsFromLeftToRight.ContainsKey(edge.TargetId))
                {
                    // part of the unchange state, Replace ids
                    edge.TargetId = switchIdsFromLeftToRight[edge.TargetId];
                }
            }
        }

        elements.AddRange(edges);
        //elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex(modelIdentifier1.Value), "Parent"));
        //elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex(modelIdentifier2.Value), "Parent"));

        return elements.ToArray();
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
            .Select(x => AsVertex(x, "AbstractState", showCompoundGraph ? "AbstractLayer" : null))
            .ToArrayAsync();

        var result2 = new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "AbstractAction"))
            .ToArrayAsync();

        var result3 = new OrientDbCommand("SELECT FROM (TRAVERSE out() FROM  (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'BlackHole'")
            .AddParameter("modelIdentifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsVertex(x, "BlackHole", showCompoundGraph ? "AbstractLayer" : null))
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
        var connections = await FetchAbstractConcreteConnectors(modelIdentifier);
        var sequenceConnections = await FetchConcreteSequenceConnectors(modelIdentifier);
        var sequenceLayer = await FetchSequenceLayerAsync(modelIdentifier, showCompoundGraph);

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
            elements.Add(new GraphElement(GraphElement.GroupNodes, new Vertex("SequenceLayer"), "Compound"));
        }

        var result1 = new OrientDbCommand("SELECT FROM TestSequence WHERE modelIdentifier = :identifier")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsVertex(x, "TestSequence", showCompoundGraph ? "SequenceLayer" : null))
            .ToArrayAsync();

        var result2 = new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').in('Accessed') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :identifier)) WHERE @class = 'SequenceNode'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsVertex(x, "SequenceNode", showCompoundGraph ? "SequenceLayer" : null))
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

    public async Task<GraphElement[]> FetchConcreteSequenceConnectors(ModelIdentifier modelIdentifier)
    {
        return await new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').inE('Accessed') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :identifier)) WHERE @class = 'Accessed'")
            .AddParameter("identifier", modelIdentifier.Value)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "Accessed"))
            .ToArrayAsync();
    }

    private static GraphElement AsEdge(JsonElement jsonElement, string typeName, params string[] extraClasses)
    {
        var id = new OrientDbId(jsonElement.GetProperty("@rid").ToString());
        var sourceId = new OrientDbId(jsonElement.GetProperty("out").ToString());
        var targetId = new OrientDbId(jsonElement.GetProperty("in").ToString());
        var jsonEdge = new Edge("e" + FormatId(id), "n" + FormatId(sourceId), "n" + FormatId(targetId));

        foreach (var property in jsonElement.EnumerateObject())
        {
            jsonEdge.AddProperty(property.Name, property.Value.ToString().Replace("\"", ""));
        }

        var element = new GraphElement(GraphElement.GroupEdges, jsonEdge, typeName);

        foreach (var extraClass in extraClasses)
        {
            element.AddClass(extraClass);
        }

        jsonEdge["uiLabel"] = element["counter"];

        return element;
    }

    // this helper method formats the @RID property into something that can be used in a web frontend
    private static string FormatId(OrientDbId id)
    {
        if (id.Id.IndexOf("#") != 0) return id.Id; // not an orientdb id
        return id.Id
            .Replace("#", "")
            .Replace(":", "_");
    }

    private GraphElement AsVertex(JsonElement jsonElement, string typeName, string? parent, params string[] extraClasses)
    {
        var id = new OrientDbId(jsonElement.GetProperty("@rid").ToString());

        var jsonVertex = new Vertex("n" + FormatId(id));

        var elementToParse = jsonElement.EnumerateObject()
            .ToList();

        foreach (var property in elementToParse)
        {
            if (property.Name == "screenshot")
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

        var element = new GraphElement(GraphElement.GroupNodes, jsonVertex, typeName);
        if (jsonElement.TryGetProperty("isInitial", out var isInitialElement) && isInitialElement.GetBoolean())
        {
            element.AddClass("isInitial");
        }

        foreach (var extraClass in extraClasses)
        {
            element.AddClass(extraClass);
        }

        if (typeName == "AbstractState")
        {
            jsonVertex["customLabel"] = element[abstractStateLabelSetting.Value];
        }
        else if (typeName == "ConcreteState")
        {
            jsonVertex["customLabel"] = element[concreteStateLabelSetting.Value];
        }
        else if (typeName == "SequenceNode")
        {
            jsonVertex["customLabel"] = element[sequenceNodeLabelSetting.Value];
        }
        else if (typeName == "TestSequence")
        {
            jsonVertex["customLabel"] = element[testSequenceLabelSetting.Value];
        }
        else
        {
            jsonVertex["customLabel"] = element["counter"];
        }

        if (showPrefixLabelSetting.Value)
        {
            if (typeName == "AbstractState")
            {
                jsonVertex["customLabel"] = new PropertyValue($"AS-{jsonVertex["customLabel"].Value}");
            }
            else if (typeName == "ConcreteState")
            {
                jsonVertex["customLabel"] = new PropertyValue($"CS-{jsonVertex["customLabel"].Value}");
            }
            else if (typeName == "SequenceNode")
            {
                jsonVertex["customLabel"] = new PropertyValue($"SN-{jsonVertex["customLabel"].Value}");
            }
            else if (typeName == "TestSequence")
            {
                jsonVertex["customLabel"] = new PropertyValue($"TS-{jsonVertex["customLabel"].Value}");
            }
            else
            {
                jsonVertex["customLabel"] = new PropertyValue($"??-{jsonVertex["customLabel"].Value}");
            }
        }

        return element;
    }
}