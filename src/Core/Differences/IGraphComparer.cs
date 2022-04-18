using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Differences;

public interface IGraphComparer
{
    Task<List<GraphElement>> Compare(Model model1, Model? model2);
}

public class GraphComparer : IGraphComparer
{
    private readonly IGraphService graphService;

    public GraphComparer(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    public async Task<List<GraphElement>> Compare(Model model1, Model? model2)
    {
        var combinedGraph = new List<GraphElement>();
        var app1Graph = await FetchGraph(model1);
        combinedGraph.AddRange(app1Graph);

        if (model2 is not null)
        {
            var app2Graph = await FetchGraph(model2);
            combinedGraph.AddRange(app2Graph);
        }

        return combinedGraph;
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