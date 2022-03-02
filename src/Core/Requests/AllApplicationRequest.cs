using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class AllApplicationRequest : IRequest<Application[]>
{
}

public class AllApplicationRequestHandler : IRequestHandler<AllApplicationRequest, Application[]>
{
    private readonly IChangeDetectionHttpClient client;

    public AllApplicationRequestHandler(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async Task<Application[]> Handle(AllApplicationRequest request, CancellationToken cancellationToken)
    {
        return await new OrientDbCommand("SELECT FROM AbstractStateModel")
            .ExecuteOn<ApplicationJson>(client)
            .Select(x => new Application
            {
                AbstractionAttributes = x.AbstractionAttributes,
                AbstractStates = Array.Empty<AbstractState>(),
                ApplicationName = x.ApplicationName,
                ApplicationVersion = x.ApplicationVersion,
                ModelIdentifier = new ModelIdentifier(x.ModelIdentifier)
            })
            .ToArrayAsync();
    }

    private class ApplicationJson
    {
        public string ModelIdentifier { get; init; }
        public string ApplicationVersion { get; init; }
        public string ApplicationName { get; set; }
        public string[] AbstractionAttributes { get; set; }
    }
}