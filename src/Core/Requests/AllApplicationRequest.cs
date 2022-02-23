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
        var command = new OrientDbCommand("SELECT FROM AbstractStateModel");
        var applications = await client.QueryAsync<ApplicationJson>(command);

        return applications
            .Select(x => new Application
            {
                AbstractionAttributes = x.AbstractionAttributes,
                AbstractStates = Array.Empty<AbstractState>(),
                ApplicationName = x.ApplicationName,
                ApplicationVersion = x.ApplicationVersion,
                ModelIdentifier = new ModelIdentifier(x.ModelIdentifier)
            })
            .ToArray();
    }

    private class ApplicationJson
    {
        public string ModelIdentifier { get; init; }
        public string ApplicationVersion { get; init; }
        public string ApplicationName { get; set; }
        public string[] AbstractionAttributes { get; set; }
    }
}