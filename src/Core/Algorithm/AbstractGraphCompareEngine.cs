using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface ICompareGraph
{
    Task<CompareResults> CompareAsync(Model oldModel, Model newModel);
}

public class AbstractGraphCompareEngine : ICompareGraph
{
    private readonly IRetrieveGraphForComparison graphRetriever;
    private readonly ICompareVertices verticesComparer;
    private readonly IStartingAbstractState startingAbstractStates;

    public AbstractGraphCompareEngine(IRetrieveGraphForComparison graphRetriever, ICompareVertices verticesComparer, IStartingAbstractState startingAbstractStates)
    {
        this.graphRetriever = graphRetriever;
        this.verticesComparer = verticesComparer;
        this.startingAbstractStates = startingAbstractStates;
    }

    public async Task<CompareResults> CompareAsync(Model oldModel, Model newModel)
    {
        // check for similar abstract attributes,
        CheckEqualAbstractAttributes(oldModel, newModel);

        // first we retrieve the complete graph from the graph server for both models
        var oldGraphApp = await graphRetriever.RetrieveAsync(oldModel);
        var newGraphApp = await graphRetriever.RetrieveAsync(newModel);

        // determine the starting vertices
        var (oldAbstractState, newAbstractState) = startingAbstractStates.Find(oldGraphApp, newGraphApp);

        // Start checking the states
        CompareAbstractStates(oldAbstractState, newAbstractState, oldGraphApp, newGraphApp);

        return new CompareResults
        {
            GraphApp1 = oldGraphApp,
            GraphApp2 = newGraphApp,
        };
    }

    private static void CheckEqualAbstractAttributes(Model oldModel, Model newModel)
    {
        var listA_in_listB = oldModel.AbstractStates.All(a => newModel.AbstractStates.Any(b => b.Equals(a)));
        var listB_in_listA = newModel.AbstractStates.All(b => oldModel.AbstractStates.Any(a => a.Equals(b)));

        if (!(listA_in_listB && listB_in_listA))
        {
            throw new ApplicationUsesDifferentAbstractAttributes(oldModel, newModel);
        }
    }

    private void CompareAbstractStates(Vertex oldAbstractState, Vertex newAbstractState, AppGraph oldGraphApp, AppGraph newGraphApp)
    {
        // set corresponding ids for each state
        newAbstractState["CD_CorrespondingId"] = oldAbstractState["stateId"];
        oldAbstractState["CD_CorrespondingId"] = newAbstractState["stateId"];

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        oldAbstractState.IsHandeld = true;
        newAbstractState.IsHandeld = true;

        verticesComparer.CompareProperties(oldAbstractState, newAbstractState);

        var abstractActionsForOldState = oldGraphApp.FindAbstractActionsFor(oldAbstractState).ToArray();
        var unhandeldActions = newGraphApp
            .FindAbstractActionsFor(newAbstractState)
            .Where(x => !x.IsHandeld)
            .ToList();

        foreach (var action in unhandeldActions)
        {
            CompareActions(action, abstractActionsForOldState, oldGraphApp, newGraphApp);
        }
    }

    private void CompareActions(Edge action, Edge[] actionsStateFromApp1, AppGraph oldGraphApp, AppGraph graphApp2)
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

            var correspondingTargetState = oldGraphApp.AbstractStates
                .First(x => x.Document.Id == correspondingAction.TargetId)
                .DocumentAsVertex();

            // first check it handling is needed
            if (!(targetState.IsHandeld || correspondingTargetState.IsHandeld))
            {
                // we now have two states that are coresponding in both graph.
                // check the differences between the two states.
                CompareAbstractStates(correspondingTargetState, targetState, oldGraphApp, graphApp2);
            }
        }
    }
}