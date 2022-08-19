using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface IRetrieveGraphForComparison
{
    Task<AppGraph> RetrieveAsync(Model model);
}

public class GraphForCompareRetriever : IRetrieveGraphForComparison
{
    private readonly IGraphService graphService;

    public GraphForCompareRetriever(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    public async Task<AppGraph> RetrieveAsync(Model model)
    {
        var appNameVersion = $"{model.Name} - {model.Version}";
        var parentNode = CreateParentNode(model, appNameVersion);
        var elements = new List<GraphElement> { parentNode };

        var abstractLayer = await graphService.FetchAbstractLayerAsync(model.ModelIdentifier, showCompoundGraph: false);
        var concreteLayer = await graphService.FetchConcreteLayerAsync(model.ModelIdentifier, showCompoundGraph: false);

        // we need to connections since we retrive the name from it for the abstract actions
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
        ForEveryAbstractStateSetTheScreenshot(appGraph);

        return appGraph;
    }

    private static GraphElement CreateParentNode(Model model, string appNameVersion)
    {
        var parent = new GraphElement(GraphElement.GroupNodes, new Vertex(appNameVersion), "Parent");
        parent[nameof(model.Name)] = new PropertyValue(model.Name);
        parent[nameof(model.Version)] = new PropertyValue(model.Version);

        return parent;
    }

    private static void ForEveryAbstractStateSetTheScreenshot(AppGraph appGraph)
    {
        foreach (var item in appGraph.AbstractStates)
        {
            var concreteStateIds = item["concreteStateIds"].AsArray();
            foreach (var id in concreteStateIds)
            {
                var screenshots = appGraph.ConcreteStates
                    .Where(x => x["stateId"] == id)
                    .Select(x => x["screenshot"].Value)
                    .ToList();

                item["screenshots"] = new PropertyValue($"[{string.Join(",", screenshots)}]");
            }
        }
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
}