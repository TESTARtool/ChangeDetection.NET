using TechTalk.SpecFlow.Assist;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Algorithm;
using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.Visualisation;

namespace GherkinTests.Algorithm;

public class NameValue
{
    public string Name { get; set; }
    public string Value { get; set; }
}

[Binding]
internal class AbstractGraphCompareEngineBindings : IRetrieveGraphForComparison
{
    private static readonly Model oldModel = new Model
    {
        AbstractionAttributes = Array.Empty<string>(),
        ModelIdentifier = new ModelIdentifier("old"),
        AbstractStates = Array.Empty<AbstractState>(),
        Name = "Old",
        Version = "1",
    };

    private static readonly Model newModel = new Model
    {
        AbstractionAttributes = Array.Empty<string>(),
        ModelIdentifier = new ModelIdentifier("new"),
        AbstractStates = Array.Empty<AbstractState>(),
        Name = "Mew",
        Version = "2",
    };

    private readonly Dictionary<int, AppGraph> graphs = new Dictionary<int, AppGraph>();
    private AppGraph? oldGraph;
    private AppGraph? newGraph;
    private AppGraph? mergeGraph;
    private CompareResults? compareResult;

    [Given(@"an abstract state (.*) in graph (.*) with the following data")]
    [Given(@"another abstract state (.*) in graph (.*) with the following data")]
    public void GivenAnAbstractStateNInGraphWithTheFollowingData(string abstrateStateId, int graphId, Table table)
    {
        var graph = GetGraphWithGraphId(graphId);
        var vertex = new Vertex(abstrateStateId);
        var data = table.CreateSet<NameValue>();
        foreach (var item in data)
        {
            vertex.AddProperty(item.Name, item.Value);
        }
        graph.Elements.Add(new GraphElement("test", vertex, "AbstractState"));
    }

    [Given(@"an egde (.*) in graph (.*) to connect verteces (.*) and (.*) with the following data")]
    public void GivenAnEgdeEInGraphToConnectVertecesNAndNWithTheFollowingData(string edgeId, int graphId, string sourceId, string targetId, Table table)
    {
        var graph = GetGraphWithGraphId(graphId);
        var edge = new Edge(edgeId, sourceId, targetId);
        var data = table.CreateSet<NameValue>();
        foreach (var item in data)
        {
            edge.AddProperty(item.Name, item.Value);
        }
        graph.Elements.Add(new GraphElement("test", edge, "AbstractAction"));
    }

    [Given(@"graph (.*) as the old graph")]
    public void GivenGraphAsTheOldGraph(int graphId)
    {
        oldGraph = GetGraphWithGraphId(graphId);
    }

    [Given(@"graph (.*) as the new graph")]
    public void GivenGraphAsTheNewGraph(int graphId)
    {
        newGraph = GetGraphWithGraphId(graphId);
    }

    [When(@"the comparison between the new and old graph has run")]
    public async Task WhenTheComparisonBetweenTheNewAndOldGraphHasRunAsync()
    {
        var starting = new InitialStartingAbstractState();
        var comparer = new CompareVertices();
        var compareEngine = new AbstractGraphCompareEngine(this, comparer, starting);
        compareResult = await compareEngine.CompareAsync(oldModel, newModel);
    }

    [When(@"the comparison result is merged")]
    public void WhenTheComparisonResultIsMerged()
    {
        var graphMerger = new MergeGraphFactory();

        mergeGraph = graphMerger.Create(compareResult);
    }

    [Then(@"merge is not null")]
    public void ThenMergeIsNotNull()
    {
        mergeGraph.Should().NotBeNull();
    }

    public Task<AppGraph> RetrieveAsync(Model model)
    {
        if (model == oldModel && oldGraph is not null)
        {
            return Task.FromResult(oldGraph);
        }

        if (model == newModel && newGraph is not null)
        {
            return Task.FromResult(newGraph);
        }

        throw new InvalidOperationException("AppGraph and or model is incorrect");
    }

    private AppGraph GetGraphWithGraphId(int graphId)
    {
        if (graphs.TryGetValue(graphId, out var graph))
        {
            return graph;
        }

        var appGraph = new AppGraph(new List<GraphElement>());

        graphs.Add(graphId, appGraph);

        return appGraph;
    }
}