using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ConcreteStateEntityRequest : IRequest<ConcreteState>
{
    public ConcreteStateId ConcreteStateId { get; init; }
}

public class ConcreteStateEntityRequestHandler : IRequestHandler<ConcreteStateEntityRequest, ConcreteState?>
{
    private readonly IChangeDetectionHttpClient client;

    public ConcreteStateEntityRequestHandler(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async Task<ConcreteState?> Handle(ConcreteStateEntityRequest request, CancellationToken cancellationToken)
    {
        var command = new OrientDbCommand("SELECT FROM ConcreteState WHERE ConcreteIDCustom :concreteIDCustom LIMIT 1")
            .AddParameter("concreteIDCustom", request.ConcreteStateId.Value);

        var concreteStates = await client.QueryAsync<ConcreteStateJson>(command);

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