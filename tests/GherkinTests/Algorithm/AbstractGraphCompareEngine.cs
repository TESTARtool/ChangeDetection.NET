using FluentAssertions.Execution;
using System.Text.Json;
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

    [AfterScenario]
    public void After()
    {
        var all = new List<GraphElement>();
        foreach (var graphCol in graphs)
        {
            all.AddRange(graphCol.Value.Elements);
        }
        var json = JsonSerializer.Serialize(all);
        File.WriteAllText("all.json", json);
    }

    [Given(@"an initial abstract state (.*) in graph (.*) with the following data")]
    public void GivenAnInitialAbstractStateInGraphWithTheFollowingData(string abstrateStateId, int graphId, Table table)
    {
        var graph = GetGraphWithGraphId(graphId);
        var vertex = new Vertex(abstrateStateId);
        var data = table.CreateSet<NameValue>();
        foreach (var item in data)
        {
            vertex.AddProperty(item.Name, item.Value);
        }
        var graphElement = new GraphElement("nodes", vertex, "AbstractState");
        vertex.AddProperty("uiname", $"{abstrateStateId}-{vertex["stateId"].Value}");
        vertex.AddProperty("parent", $"GraphId{graphId}");
        graphElement.Classes.Add("isInitial");
        graph.Elements.Add(graphElement);
    }

    [Given(@"an abstract state (.*) in graph (.*) with the following data")]
    [Given(@"another abstract state (.*) in graph (.*) with the following data")]
    public void GivenAnAbstractStateInGraphWithTheFollowingData(string abstrateStateId, int graphId, Table table)
    {
        var graph = GetGraphWithGraphId(graphId);
        var vertex = new Vertex(abstrateStateId);
        var data = table.CreateSet<NameValue>();
        foreach (var item in data)
        {
            vertex.AddProperty(item.Name, item.Value);
        }
        vertex.AddProperty("uiname", $"{abstrateStateId}-{vertex["stateId"].Value}");
        vertex.AddProperty("parent", $"GraphId{graphId}");
        graph.Elements.Add(new GraphElement("nodes", vertex, "AbstractState"));
    }

    [Given(@"an egde (.*) in graph (.*) to connect verteces (.*) and (.*) with the following data")]
    public void GivenAnEgdeInGraphToConnectVertecesAndWithTheFollowingData(string edgeId, int graphId, string sourceId, string targetId, Table table)
    {
        var graph = GetGraphWithGraphId(graphId);
        var edge = new Edge(edgeId, sourceId, targetId);
        var data = table.CreateSet<NameValue>();
        foreach (var item in data)
        {
            edge.AddProperty(item.Name, item.Value);
        }
        edge.AddProperty("uiname", $"{edgeId}-{edge["actionId"].Value}");
        graph.Elements.Add(new GraphElement("edges", edge, "AbstractAction"));
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
        compareResult.Should().NotBeNull();
        var graphMerger = new MergeGraphFactory();

        mergeGraph = graphMerger.Create(compareResult!);
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

    [Then(@"the merge contains (.*) abstract states and (.*) abstract actions")]
    [Then(@"the merge contains (.*) abstract states and (.*) abstract action")]
    public void ThenTheMergeContainsStatesAndActions(int statesCount, int actionCount)
    {
        mergeGraph.Should().NotBeNull();
        mergeGraph!.AbstractStates.Should().HaveCount(statesCount);
        mergeGraph!.AbstractActions.Should().HaveCount(actionCount);
    }

    [Then(@"the initial abstract state has the following data")]
    public void ThenTheInitialAbstractStateHasTheFollowingData(Table table)
    {
        var initialState = mergeGraph!.AbstractStates.First(x => x.IsInitial);
        var data = table.CreateSet<NameValue>();
        var props = initialState.Document.Properties;
        using (new AssertionScope())
        {
            foreach (var item in data)
            {
                initialState[item.Name].Value.Should().Be(item.Value);
            }
        }
    }

    [Then(@"abstract state with stateId (.*) has the following data")]
    public void ThenAbstractStateWithStateIdHasTheFollowingData(string abstractStateId, Table table)
    {
        var state = mergeGraph!.AbstractStates.FirstOrDefault(x => x["stateId"].Value == abstractStateId);
        state.Should().NotBeNull();

        var data = table.CreateSet<NameValue>();
        var props = state!.Document.Properties;
        using (new AssertionScope())
        {
            foreach (var item in data)
            {
                state[item.Name].Value.Should().Be(item.Value);
            }
        }
    }

    [Then(@"abstract state with stateId (.*) is not included in the merge graph")]
    public void ThenAbstractStateWithStateIdIsNotIncludedInTheMergeGraph(string abstractStateId)
    {
        var state = mergeGraph!.AbstractStates.FirstOrDefault(x => x["stateId"].Value == abstractStateId);
        state.Should().BeNull();
    }

    [Then(@"abstract state with stateId (.*) has the following class")]
    public void ThenAbstractStateWithStateIdHasTheFollowingClass(string abstractStateId, Table table)
    {
        var state = mergeGraph!.AbstractStates.FirstOrDefault(x => x["stateId"].Value == abstractStateId);
        state.Should().NotBeNull();

        var classNames = table.CreateSet<string>(x => x["ClassName"].ToString());
        using (new AssertionScope())
        {
            foreach (var className in classNames)
            {
                state!.Classes.Should().Contain(className);
            }
        }
    }

    private AppGraph GetGraphWithGraphId(int graphId)
    {
        if (graphs.TryGetValue(graphId, out var graph))
        {
            return graph;
        }

        var appGraph = new AppGraph(new List<GraphElement>());

        var compound = new GraphElement("nodes", new Vertex($"GraphId{graphId}"), $"Compound");
        compound.Document.AddProperty("uiname", $"Graph {graphId}");
        appGraph.Elements.Add(compound);
        graphs.Add(graphId, appGraph);

        return appGraph;
    }
}