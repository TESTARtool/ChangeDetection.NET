using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Differences;

public class CompareResults
{
    public List<GraphElement> GraphApp1 { get; set; }
    public List<GraphElement> GraphApp2 { get; set; }
}

public class AppGraph
{
    public AppGraph(List<GraphElement> elements)
    {
        Elements = elements;
    }

    public List<GraphElement> Elements { get; }
    public IEnumerable<GraphElement> AbstractStates => Elements.Where(x => x.IsAbstractState);
    public IEnumerable<GraphElement> AbstractActions => Elements.Where(x => x.IsAbstractAction);
    public IEnumerable<GraphElement> ConcreteActions => Elements.Where(x => x.IsConcreteAction);
    public IEnumerable<GraphElement> ConcreteStates => Elements.Where(x => x.IsConcreteState);
    public bool ContainsUnhandledAbstractStates => AbstractStates.Any(x => !x.IsHandeld);

    public Vertex? InitialAbstractState => AbstractStates.FirstOrDefault(x => x.IsInitial)?.DocumentAsVertex;

    public IEnumerable<Edge> FindAbstractActionsFor(Vertex state)
    {
        return AbstractActions.Where(x => x.Document.SourceId == state.Id);
    }
}

public interface IGraphComparer
{
    Task<CompareResults> Compare(Model model1, Model model2);
}

public class GraphComparer : IGraphComparer
{
    private readonly IGraphService graphService;

    public GraphComparer(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    public async Task<CompareResults> Compare(Model model1, Model model2)
    {
        // first we retrieve the complete graph from the graph server for both models
        var graphApp1 = await FetchGraph(model1);
        var graphApp2 = await FetchGraph(model2);

        // we will start from the initial value and asume it is the same vertex
        // the initial vertex may still be different offcourse, but we need to
        // get an inital start.

        // prehaps later we can find a starting point when the initial
        // vertex is not the same

        var initalStateApp1 = graphApp1.InitialAbstractState;

        if (initalStateApp1 is null)
        {
            throw new ComparisonException("Unable to find abstract state in graph app 1");
        }

        var initalStateApp2 = graphApp2.InitialAbstractState;

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        initalStateApp1.IsHandeld = true;
        initalStateApp2.IsHandeld = true;

        FF(graphApp1, graphApp2, initalStateApp1, initalStateApp2);

        return new CompareResults
        {
            GraphApp1 = graphApp1.Elements,
            GraphApp2 = graphApp2.Elements,
        };
    }

    private void FF(AppGraph graphApp1, AppGraph graphApp2, Vertex abstractState1, Vertex abstractState2)
    {
        var actionsStateApp1 = graphApp1.FindAbstractActionsFor(abstractState1);
        var actionsStateApp2 = graphApp2.FindAbstractActionsFor(abstractState2);

        foreach (var action in actionsStateApp2)
        {
            if (!action.IsHandeld)
            {
                FindDifferences(action, actionsStateApp1, graphApp1, graphApp2);
            }
        }
    }

    private void FindDifferences(Edge action, IEnumerable<Edge> actionsStateApp1, AppGraph graphApp1, AppGraph graphApp2)
    {
        // always mark as handeld for prevent double handling
        action.IsHandeld = true;

        var correspondingAction = actionsStateApp1.FirstOrDefault(x => x["actionId"] == action["actionId"]);
        if (correspondingAction is null)
        {
            // this must be a new or altered action since a corresponding action
            // was not found in the abstract state.
            // not the first version lets mark it as new

            action["CD_CompareResult"] = "new";
        }
        else
        {
            // corresponding action found. continue with outgoing state
            correspondingAction.IsHandeld = true;

            var targetState = graphApp2.AbstractStates
                .First(x => x.Document.Id == action["target"])
                .DocumentAsVertex;

            var correspondingTargetState = graphApp1.AbstractStates
                .First(x => x.Document.Id == correspondingAction["target"])
                .DocumentAsVertex;

            // first check it handling is needed
            if (!(targetState.IsHandeld || correspondingTargetState.IsHandeld))
            {
                // we now have two states that are coresponding in each graph.
                targetState.IsHandeld = true;
                correspondingTargetState.IsHandeld = true;

                // check the differnces.

                FF(graphApp1, graphApp2, targetState, correspondingTargetState);
            }
        }
    }

    private void ForEveryAbstractActionSetTheDescriptionFromConcreteAction(AppGraph appGraph)
    {
        foreach (var item in appGraph.AbstractActions)
        {
            var concreteActionIds = ParseArray(item["concreteActionIds"]);

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

    private string[] ParseArray(string value) => value.ToString().Replace("[", "").Replace("]", "").Split(',');

    private async Task<AppGraph> FetchGraph(Model model)
    {
        var appNameVersion = $"{model.Name} - {model.Version}";
        var parent = new GraphElement(GraphElement.GroupNodes, new Vertex(appNameVersion), "Parent");
        parent.Document.AddProperty(nameof(model.Name), model.Name);
        parent.Document.AddProperty(nameof(model.Version), model.Version);

        var elements = new List<GraphElement>
        {
            parent
        };

        var abstractLayer = await graphService.FetchAbstractLayerAsync(model.ModelIdentifier, false);

        //if (abstractLayer.Any(x => x.Classes.Any(y => y == "BlackHole")))
        //{
        //    throw new ApplicationContainsBlackHoleException(model);
        //}

        var concreteLayer = await graphService.FetchConcreteLayerAsync(model.ModelIdentifier, false);
        var connections = await graphService.FetchAbstractConcreteConnectors(model.ModelIdentifier);

        elements.AddRange(abstractLayer);
        elements.AddRange(concreteLayer);
        elements.AddRange(connections);

        foreach (var element in elements)
        {
            if (element.Document is Vertex)
            {
                element.Document.AddProperty("parent", appNameVersion);
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

public class ApplicationContainsBlackHoleException : Exception
{
    private readonly Model model;

    public ApplicationContainsBlackHoleException(Model model)
    {
        this.model = model;
    }
}

public class ComparisonException : Exception
{
    public ComparisonException(string message) : base(message)
    {
    }
}