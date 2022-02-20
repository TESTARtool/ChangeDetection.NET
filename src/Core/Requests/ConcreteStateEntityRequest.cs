using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ConcreteStateEntityRequest : IRequest<ConcreteState>
{
    public ConcreteStateId ConcreteStateId { get; init; }
}

public class ConcreteStateEntityRequestHandler : IRequestHandler<ConcreteStateEntityRequest, ConcreteState?>
{
    private readonly IOrientDbCommandExecuter orientDbCommand;

    public ConcreteStateEntityRequestHandler(IOrientDbCommandExecuter orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task<ConcreteState?> Handle(ConcreteStateEntityRequest request, CancellationToken cancellationToken)
    {
        var sql = $"SELECT FROM ConcreteState WHERE ConcreteIDCustom = '{request.ConcreteStateId.Value}' LIMIT 1";

        var concreteStates = await orientDbCommand.ExecuteQueryAsync<ConcreteStateJson>(sql);

        return concreteStates.Select(x => new ConcreteState
        {
            ConcreteIDCustom = new ConcreteIDCustom(x.ConcreteIDCustom),
            Screenshot = new OrientDbId(x.Screenshot),
        }).FirstOrDefault();
    }

    public class ConcreteStateJson
    {
        public string ConcreteIDCustom { get; set; }
        public string Screenshot { get; set; }
    }
}