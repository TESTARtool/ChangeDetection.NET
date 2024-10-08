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
        return await new OrientDbCommand("SELECT FROM ConcreteState WHERE ConcreteID = :concreteID LIMIT 1")
            .AddParameter("concreteID", request.ConcreteStateId.Value)
            .ExecuteOn<ConcreteStateJson>(client)
            .Select(x => new ConcreteState
            {
                ConcreteID = new ConcreteID(x.ConcreteID),
                Screenshot = new OrientDbId(x.Screenshot),
            })
            .FirstOrDefaultAsync();
    }

    public class ConcreteStateJson
    {
        public string ConcreteID { get; set; }
        public string Screenshot { get; set; }
    }
}