using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface ICompareGraph
{
    Task<CompareResults> CompareAsync(Model model1, Model model2);
}

public class GraphCompareEngine : ICompareGraph
{
    private readonly IRetrieveGraphForComparison graphRetriever;
    private readonly ICompareVertices verticesComparer;
    private readonly IStartingAbstractState startingAbstractStates;

    public GraphCompareEngine(IRetrieveGraphForComparison graphRetriever, ICompareVertices verticesComparer, IStartingAbstractState startingAbstractStates)
    {
        this.graphRetriever = graphRetriever;
        this.verticesComparer = verticesComparer;
        this.startingAbstractStates = startingAbstractStates;
    }

    public async Task<CompareResults> CompareAsync(Model model1, Model model2)
    {
        // first we retrieve the complete graph from the graph server for both models
        var graphApp1 = await graphRetriever.RetrieveAsync(model1);
        var graphApp2 = await graphRetriever.RetrieveAsync(model2);

        // determine the starting vertices
        var abstractState1 = startingAbstractStates.Find(graphApp1);
        var abstractState2 = startingAbstractStates.Find(graphApp2);

        // Start checking the states
        CompareAbstractStates(abstractState1, abstractState2, graphApp1, graphApp2);

        return new CompareResults
        {
            GraphApp1 = graphApp1,
            GraphApp2 = graphApp2,
        };
    }

    private void CompareAbstractStates(Vertex abstractState1, Vertex abstractState2, AppGraph graphApp1, AppGraph graphApp2)
    {
        // set corresponding ids for each state
        abstractState2["CD_CorrespondingId"] = abstractState1["stateId"];
        abstractState1["CD_CorrespondingId"] = abstractState2["stateId"];

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        abstractState1.IsHandeld = true;
        abstractState2.IsHandeld = true;

        verticesComparer.CompareProperties(abstractState1, abstractState2);

        var actionsStateApp1 = graphApp1.FindAbstractActionsFor(abstractState1).ToArray();
        var unhandeldActions = graphApp2.FindAbstractActionsFor(abstractState2)
            .Where(x => !x.IsHandeld)
            .ToList();

        foreach (var action in unhandeldActions)
        {
            CompareActions(action, actionsStateApp1, graphApp1, graphApp2);
        }
    }

    private void CompareActions(Edge action, Edge[] actionsStateFromApp1, AppGraph graphApp1, AppGraph graphApp2)
    {
        // always mark as handeld for prevent double handling
        action.IsHandeld = true;

        var correspondingAction = actionsStateFromApp1.FirstOrDefault(x => x["actionId"] == action["actionId"]);
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
                .DocumentAsVertex();

            var correspondingTargetState = graphApp1.AbstractStates
                .First(x => x.Document.Id == correspondingAction.TargetId)
                .DocumentAsVertex();

            // first check it handling is needed
            if (!(targetState.IsHandeld || correspondingTargetState.IsHandeld))
            {
                // we now have two states that are coresponding in both graph.
                targetState.IsHandeld = true;
                correspondingTargetState.IsHandeld = true;

                // check the differences between the two states.
                CompareAbstractStates(correspondingTargetState, targetState, graphApp1, graphApp2);
            }
        }
    }
}