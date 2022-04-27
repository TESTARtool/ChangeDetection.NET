using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Differences;

public class CompareResults
{
    public List<GraphElement> GraphApp1 { get; set; }
    public List<GraphElement> GraphApp2 { get; set; }
}

public interface IGraphComparer
{
    Task<CompareResults> Compare(Model model1, Model model2);
}

public class GraphComparer : IGraphComparer
{
    private readonly IGraphService graphService;
    private readonly ICompareVertices verticesComparer;
    private readonly IStartingAbstractState startingAbstractStates;

    public GraphComparer(IGraphService graphService, ICompareVertices verticesComparer, IStartingAbstractState startingAbstractStates)
    {
        this.graphService = graphService;
        this.verticesComparer = verticesComparer;
        this.startingAbstractStates = startingAbstractStates;
    }

    public async Task<CompareResults> Compare(Model model1, Model model2)
    {
        // first we retrieve the complete graph from the graph server for both models
        var graphApp1 = await FetchGraph(model1);
        var graphApp2 = await FetchGraph(model2);

        // then we get some starting states
        var (abstractState1, abstractState2) = startingAbstractStates.DetemineStartingStates(graphApp1, graphApp2);

        // lets start checking the states
        CheckStateDifferences(graphApp1, graphApp2, abstractState1, abstractState2);

        // report the results
        return new CompareResults
        {
            GraphApp1 = graphApp1.Elements,
            GraphApp2 = graphApp2.Elements,
        };
    }

    private static void ForEveryAbstractActionSetTheDescriptionFromConcreteAction(AppGraph appGraph)
    {
        foreach (var item in appGraph.AbstractActions)
        {
            var concreteActionIds = item["concreteActionIds"].AsArray();

            foreach (var id in concreteActionIds)
            {
                var firstConcreteAction = appGraph.ConcreteActions.FirstOrDefault(x => x["actionId"] == id);
                if (firstConcreteAction is not null)
                {
                    item["Description"] = firstConcreteAction["Desc"];
                    break;
                }
            }
        }
    }

    private void CheckStateDifferences(AppGraph graphApp1, AppGraph graphApp2, Vertex abstractState1, Vertex abstractState2)
    {
        // set corresponding ids for each state
        abstractState2["CD_CorrespondingId"] = abstractState1["stateId"];
        abstractState1["CD_CorrespondingId"] = abstractState2["stateId"];

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        abstractState1.IsHandeld = true;
        abstractState2.IsHandeld = true;

        verticesComparer.CompareProperties(abstractState1, abstractState2);

        var actionsStateApp1 = graphApp1.FindAbstractActionsFor(abstractState1);
        var unhandeldActions = graphApp2.FindAbstractActionsFor(abstractState2)
            .Where(x => !x.IsHandeld);

        foreach (var action in unhandeldActions)
        {
            FindDifferencesFromAction(action, actionsStateApp1, graphApp1, graphApp2);
        }
    }

    private void FindDifferencesFromAction(Edge action, IEnumerable<Edge> actionsStateApp1, AppGraph graphApp1, AppGraph graphApp2)
    {
        // always mark as handeld for prevent double handling
        action.IsHandeld = true;

        var correspondingAction = actionsStateApp1.FirstOrDefault(x => x["actionId"] == action["actionId"]);
        if (correspondingAction is null)
        {
            // this must be a new or altered action since a corresponding action
            // was not found in the abstract state.
            // not the first version lets mark it as new

            action["CD_CompareResult"] = new PropertyValue("new");
        }
        else
        {
            // corresponding action found. continue with outgoing state
            correspondingAction.IsHandeld = true;

            var targetState = graphApp2.AbstractStates
                .First(x => x.Document.Id == action.TargetId)
                .DocumentAsVertex;

            var correspondingTargetState = graphApp1.AbstractStates
                .First(x => x.Document.Id == correspondingAction.TargetId)
                .DocumentAsVertex;

            // first check it handling is needed
            if (!(targetState.IsHandeld || correspondingTargetState.IsHandeld))
            {
                // we now have two states that are coresponding in each graph.
                targetState.IsHandeld = true;
                correspondingTargetState.IsHandeld = true;

                // check the differnces.
                CheckStateDifferences(graphApp1, graphApp2, targetState, correspondingTargetState);
            }
        }
    }

    private async Task<AppGraph> FetchGraph(Model model)
    {
        var appNameVersion = $"{model.Name} - {model.Version}";
        var parent = new GraphElement(GraphElement.GroupNodes, new Vertex(appNameVersion), "Parent");
        parent[nameof(model.Name)] = new PropertyValue(model.Name);
        parent[nameof(model.Version)] = new PropertyValue(model.Version);

        var elements = new List<GraphElement>
        {
            parent
        };

        var abstractLayer = await graphService.FetchAbstractLayerAsync(model.ModelIdentifier, showCompoundGraph: false);
        var concreteLayer = await graphService.FetchConcreteLayerAsync(model.ModelIdentifier, showCompoundGraph: false);
        var connections = await graphService.FetchAbstractConcreteConnectors(model.ModelIdentifier);

        elements.AddRange(abstractLayer);
        elements.AddRange(concreteLayer);
        elements.AddRange(connections);

        foreach (var element in elements)
        {
            if (element.Document is Vertex)
            {
                element.Document["parent"] = new PropertyValue(appNameVersion);
            }

            if (element.Document is Edge)
            {
                element["uiLabel"] = element["actionId"];
            }
        }

        var appGraph = new AppGraph(elements);

        // this information is not present in the abstract action
        // so find for each abstract action a description.
        ForEveryAbstractActionSetTheDescriptionFromConcreteAction(appGraph);

        return appGraph;
    }
}