using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public class AppGraph
{
    public AppGraph(IEnumerable<GraphElement> elements)
    {
        Elements = elements.ToList();
    }

    public List<GraphElement> Elements { get; }
    public IEnumerable<GraphElement> AbstractStates => Elements.Where(x => x.IsAbstractState);
    public IEnumerable<GraphElement> AbstractActions => Elements.Where(x => x.IsAbstractAction);
    public IEnumerable<GraphElement> ConcreteActions => Elements.Where(x => x.IsConcreteAction);
    public IEnumerable<GraphElement> ConcreteStates => Elements.Where(x => x.IsConcreteState);
    public bool ContainsUnhandledAbstractStates => AbstractStates.Any(x => !x.IsHandled);

    public Vertex? InitialAbstractState => AbstractStates.FirstOrDefault(x => x.IsInitial)?.DocumentAsVertex();

    public IEnumerable<Edge> FindAbstractActionsFromSource(Vertex state)
    {
        return AbstractActions
            .Where(x => x.Document.SourceId == state.Id)
            .Select(x => x.DocumentAsEdge());
    }

    public IEnumerable<Edge> FindAbstractActionsForTarget(Vertex state)
    {
        return AbstractActions
            .Where(x => x.Document.TargetId == state.Id)
            .Select(x => x.DocumentAsEdge());
    }
}