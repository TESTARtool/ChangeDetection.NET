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

    public GraphComparer(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    public async Task<CompareResults> Compare(Model model1, Model model2)
    {
        return new CompareResults
        {
            GraphApp1 = await FetchGraph(model1),
            GraphApp2 = await FetchGraph(model2)
        };
    }

    private async Task<List<GraphElement>> FetchGraph(Model model)
    {
        var appNameVersion = $"{model.Name} - {model.Version}";
        var appGraph = new List<GraphElement>
        {
            new GraphElement(GraphElement.GroupNodes, new Vertex(appNameVersion), "Parent")
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