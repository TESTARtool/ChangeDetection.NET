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

    public GraphElement InitialAbstractState => Elements.First(x => x.IsInitial);

    public IEnumerable<GraphElement> FindAbstractActionsFor(GraphElement state)
    {
        return AbstractActions.Where(x => x.Document.SourceId == state.Document.Id);
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

        // this information is not present in the abstract action
        // so find for each abstract action a description.
        ForEveryAbstractActionSetTheDescriptionFromConcreteAction(graphApp1);
        ForEveryAbstractActionSetTheDescriptionFromConcreteAction(graphApp2);

        // we will start from the initial value and asume it is the same vertex
        // the initial vertex may still be different offcourse, but we need to
        // get an inital start.

        // prehaps later we can find a starting point when the initial
        // vertex is not the same

        var initalStateApp1 = graphApp1.InitialAbstractState;
        var initalStateApp2 = graphApp2.InitialAbstractState;

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        initalStateApp1.IsHandeld = true;
        initalStateApp2.IsHandeld = true;

        var actionsStateApp1 = graphApp1.FindAbstractActionsFor(initalStateApp1);
        var actionsStateApp2 = graphApp2.FindAbstractActionsFor(initalStateApp2);

        foreach (var action in actionsStateApp2)
        {
        }

        return new CompareResults
        {
            GraphApp1 = graphApp1.Elements,
            GraphApp2 = graphApp2.Elements,
        };
    }

    private void FindDifferences(GraphElement abstractState, AppGraph appGraph2)
    {
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

        return new AppGraph(elements);
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