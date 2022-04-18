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

    private string[] ParseArray(string value) => value.ToString().Replace("[", "").Replace("]", "").Split(',');

    public async Task<CompareResults> Compare(Model model1, Model model2)
    {
        var graphApp1 = await FetchGraph(model1);
        var graphApp2 = await FetchGraph(model2);

        var abstractStates = graphApp1.Where(x => x.IsAbstractState);
        var abstractActions = graphApp1.Where(x => x.IsAbstractAction);

        foreach (var abstractState in abstractStates)
        {
            var uy = "concreteActionIds";
            var id = abstractState.Document.Id;
            var outgoingActions = abstractActions.Where(x => x.Document.SourceId == id);


        }

        return new CompareResults
        {
            GraphApp1 = graphApp1,
            GraphApp2 = graphApp2
        };
    }

    private async Task<List<GraphElement>> FetchGraph(Model model)
    {
        var appNameVersion = $"{model.Name} - {model.Version}";
        var parent = new GraphElement(GraphElement.GroupNodes, new Vertex(appNameVersion), "Parent");
        parent.Document.AddProperty(nameof(model.Name), model.Name);
        parent.Document.AddProperty(nameof(model.Version), model.Version);

        var appGraph = new List<GraphElement>
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

        appGraph.AddRange(abstractLayer);
        appGraph.AddRange(concreteLayer);
        appGraph.AddRange(connections);

        foreach (var element in appGraph)
        {
            if (element.Document is Vertex)
            {
                element.Document.AddProperty("parent", appNameVersion);
            }
        }

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