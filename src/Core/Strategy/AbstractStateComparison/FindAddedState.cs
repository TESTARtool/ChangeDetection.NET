using MediatR;
using Testar.ChangeDetection.Core.Requests;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public enum ActionType
{
    Incomming,
    Outgoing
}

public class ApplicationDifferences
{
    public List<DeltaState> AddedStates { get; } = new();

    public List<DeltaState> RemovedStates { get; } = new();

    public void AddAddedState(AbstractStateId stateId, IEnumerable<ConcreteStateEntity> concreteState, List<DeltaAction> outgoingDeltaActions, List<DeltaAction> incommingDeltaActions)
    {
        AddedStates.Add(new DeltaState
        {
            StateId = stateId,
            ConcreteStates = concreteState,
            incommingDeltaActions = incommingDeltaActions,
            OutgoingDeltaActions = outgoingDeltaActions,
        });
    }

    public void AddRemovedState(AbstractStateId stateId, IEnumerable<ConcreteStateEntity> concreteState, List<DeltaAction> outgoingDeltaActions, List<DeltaAction> incommingDeltaActions)
    {
        RemovedStates.Add(new DeltaState
        {
            StateId = stateId,
            ConcreteStates = concreteState,
            incommingDeltaActions = incommingDeltaActions,
            OutgoingDeltaActions = outgoingDeltaActions,
        });
    }
}

public interface IFindStateChanges
{
    Task<DeltaState[]> FindStateChanges(Application control, Application test);
}

public class FindAddedState
{
    private readonly IMediator mediator;

    public FindAddedState(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<ApplicationDifferences> FindStateChanges(Application control, Application test)
    {
        var abstractStatesEqual = control.AbstractionAttributes.SequenceEqual(test.AbstractionAttributes);

        if (!abstractStatesEqual)
        {
            // if Abstract Attributes are different, Abstract Layer is different and no sense to continue
            throw new AbstractAttributesNotTheSameException();
        }

        var applicationDifferences = new ApplicationDifferences();

        var addedStates = test.AbstractStates
            .Where(x => !control.AbstractStates.Select(y => y.StateId).Contains(x.StateId))
            .ToList();

        var removeStates = control.AbstractStates
            .Where(x => !test.AbstractStates.Select(y => y.StateId).Contains(x.StateId))
            .ToList();

        // we now know which ABSTRACT state is removed from application2 and which abstract state has been added in application2
        // we need to map the abstract states to concrete state. We now we assume that with the removal of remove state
        // the concrete state are also removed.

        foreach (var addedState in addedStates)
        {
            var concreteState = await GetConcreteStateEntities(addedState);
            var outgoingDeltaActions = await GetDeltaAction(addedState.OutAbstractActions, ActionType.Outgoing);
            var incommingDeltaActions = await GetDeltaAction(addedState.OutAbstractActions, ActionType.Incomming);

            applicationDifferences.AddAddedState(addedState.StateId, concreteState, outgoingDeltaActions, incommingDeltaActions);
        }

        foreach (var removedState in removeStates)
        {
            var concreteState = await GetConcreteStateEntities(removedState);
            var outgoingDeltaActions = await GetDeltaAction(removedState.OutAbstractActions, ActionType.Outgoing);
            var incommingDeltaActions = await GetDeltaAction(removedState.OutAbstractActions, ActionType.Incomming);

            applicationDifferences.AddRemovedState(removedState.StateId, concreteState, outgoingDeltaActions, incommingDeltaActions);
        }

        return applicationDifferences;
    }

    private async Task<List<DeltaAction>> GetDeltaAction(AbstractActionId[] actionIds, ActionType actionType)
    {
        var returns = new List<DeltaAction>();

        // for the delta action we need to retrieve the Description from a ConcreteAction
        // Every AbstractAction must have a corresponding concrete action

        // we have the id of the action -> find the abstract action

        foreach (var id in actionIds)
        {
            var request = new ApplicationActionRequest { AbstractActionId = id };
            var actions = (await mediator.Send(request))
                .Select(x => new DeltaAction
                {
                    ActionId = id,
                    Description = x.Description,
                    ActionType = actionType,
                }).ToList();

            returns.AddRange(actions);
        }

        return returns;
    }

    private async Task<IEnumerable<ConcreteStateEntity>> GetConcreteStateEntities(AbstractState abstractState)
    {
        var returns = new List<ConcreteStateEntity>();
        foreach (var id in abstractState.ConcreteStateIds)
        {
            var concreteState = await mediator.Send(new ConcreteStateEntityRequest { ConcreteStateId = id });
            if (concreteState is not null)
            {
                returns.Add(concreteState);
            }
        }

        return returns;
    }
}

public class DeltaState
{
    public AbstractStateId StateId { get; set; }
    public IEnumerable<ConcreteStateEntity> ConcreteStates { get; set; }
    public List<DeltaAction> OutgoingDeltaActions { get; set; }
    public List<DeltaAction> incommingDeltaActions { get; set; }
}

public class DeltaAction
{
    public AbstractActionId ActionId { get; set; }
    public string Description { get; set; }
    public ActionType ActionType { get; set; }
}