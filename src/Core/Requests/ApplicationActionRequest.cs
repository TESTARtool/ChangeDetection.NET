using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ApplicationActionRequest : IRequest<AbstractAction[]>
{
    public OrientDbId AbstractActionId { get; set; }
}

public class ActionRequestHandler : IRequestHandler<ApplicationActionRequest, AbstractAction[]>
{
    private readonly IChangeDetectionHttpClient client;

    public ActionRequestHandler(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async Task<AbstractAction[]> Handle(ApplicationActionRequest request, CancellationToken cancellationToken)
    {
        var action = await AbstractAction(request);

        var actions = new List<AbstractAction>();

        foreach (var concreteActionId in action.ConcreteActionIds)
        {
            var concreteActions = new OrientDbCommand("SELECT `Desc` FROM ConcreteAction WHERE actionId = :actionId")
                .AddParameter("actionId", concreteActionId)
                .ExecuteOn<ConcreteActionJson>(client)
                .Select(x => new AbstractAction
                {
                    AbstractActionId = new AbstractActionId(action.ActionId),
                    ConcreteActionId = new ConcreteActionId(concreteActionId),
                    Description = x.Desc,
                })
                .ToListAsync();

            actions.AddRange(await concreteActions);
        }

        return actions.ToArray();
    }

    public async Task<ActionJson> AbstractAction(ApplicationActionRequest request)
    {
        var action = await new OrientDbCommand("SELECT FROM AbstractAction WHERE @rid = :rid")
            .AddParameter("rid", request.AbstractActionId.Id.Replace("#", "").Trim())
            .ExecuteOn<ActionJson>(client)
            .SingleOrDefaultAsync();

        return action ?? throw new Exception($"Unable to find actions with Id '{request.AbstractActionId.Id}'");
    }

    public class ActionJson
    {
        public string ActionId { get; set; }
        public string[] ConcreteActionIds { get; set; }
    }

    public class ConcreteActionJson
    {
        public string Desc { get; set; }
    }
}