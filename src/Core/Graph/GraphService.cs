using System.Text.Json;
using Testar.ChangeDetection.Core.Settings;

namespace Testar.ChangeDetection.Core.Graph;

public interface IScreenshotService
{
    Task<Base64> DownloadScreenshotAsync(string id);
}

public class ScreenshotService : IScreenshotService
{
    private readonly IChangeDetectionHttpClient httpClient;

    public ScreenshotService(IChangeDetectionHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<Base64> DownloadScreenshotAsync(string id)
    {
        var screenshot = await httpClient.DocumentAsBase64Async(new OrientDbId(id.Replace('_', ':')));

        return new Base64(screenshot ?? string.Empty);
    }
}

public record Base64(string Value)
{
}

public interface IGraphService
{
    Task<GraphElement[]> FetchGraph(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchConcreteLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchAbstractConcreteConnectors(ModelIdentifier modelIdentifier);

    Task<GraphElement[]> FetchSequenceLayerAsync(ModelIdentifier modelIdentifier, bool showCompoundGraph);

    Task<GraphElement[]> FetchConcreteSequenceConnectors(ModelIdentifier modelIdentifier);

    Task<GraphElement[]> FetchWidgetTreeGraph(OrientDbId rid);

    GraphElement[] MergeMultipleEdgesIntoOneEdge(GraphElement[] graphElements);
}

public class GraphService : IGraphService
{
    private readonly IChangeDetectionHttpClient httpClient;
    private readonly AbstractStateLabelSetting abstractStateLabelSetting;
    private readonly TestSequenceLabelSetting testSequenceLabelSetting;
    private readonly SequenceNodeLabelSetting sequenceNodeLabelSetting;
    private readonly ConcreteStateLabelSetting concreteStateLabelSetting;
    private readonly ShowPrefixLabelSettings showPrefixLabelSetting;
    private readonly JsonSerializerOptions options;

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

        options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public GraphElement[] MergeMultipleEdgesIntoOneEdge(GraphElement[] graphElements)
    {
        var newElements = graphElements.Where(x => x.Document is not Edge).ToList();

        var groupedEdges = graphElements
            .Where(x => x.Document is Edge)
            .GroupBy(x => string.Concat(x.Document.TargetId, "-", x.Document.SourceId))
            .ToList();

        var counter = 1;

        foreach (var group in groupedEdges)
        {
            if (group.Count() > 1)
            {
                var firstGraph = group.First();

                var document = new Edge($"em{counter}", firstGraph.Document.SourceId!, firstGraph.Document.TargetId!);

                foreach (var element in group)
                {
                    var json = JsonSerializer.Serialize(element.Document.Properties, options);
                    document.AddProperty(element.Document.Id, json);
                }

                counter++;

                var graphElement = new GraphElement("edges", document, firstGraph.TypeName);
                graphElement.Classes = firstGraph.Classes;
                graphElement.AddClass("MergedEdges");

                newElements.Add(graphElement);
            }
            else
            {
                newElements.Add(group.First());
            }
        }

        return newElements.ToArray();
    }

    public async Task<GraphElement[]> FetchWidgetTreeGraph(OrientDbId rid)
    {
        List<GraphElement> elements = new();

        var widgets = new OrientDbCommand("SELECT FROM(TRAVERSE IN('isChildOf') FROM(SELECT FROM Widget WHERE @RID = :rid))")
            .AddParameter("rid", rid.Id)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsVertex(x, "Widget", null, rid.Id))
            .ToArrayAsync();

        var edges = new OrientDbCommand("SELECT FROM isChildOf WHERE in IN(SELECT @RID FROM (TRAVERSE in('isChildOf') FROM (SELECT FROM Widget WHERE @RID = :rid)))")
            .AddParameter("rid", rid.Id)
            .ExecuteOn<JsonElement>(httpClient)
            .Select(x => AsEdge(x, "isChildOf"))
            .ToArrayAsync();

        elements.AddRange(await widgets);
        elements.AddRange(await edges);

        return elements.ToArray();
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