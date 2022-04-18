using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Differences;
using Testar.ChangeDetection.Core.Services;

namespace Testar.ChangeDetection.ConsoleApp.Scenarios;

public class ComparisonScenario : IScenario
{
    private readonly IChangeDetectionHttpClient client;
    private readonly IGraphComparer graphComparer;
    private readonly IModelService modelService;

    public ComparisonScenario(IChangeDetectionHttpClient client, IGraphComparer graphComparer, IModelService modelService)
    {
        this.client = client;
        this.graphComparer = graphComparer;
        this.modelService = modelService;
    }

    public async Task RunAsync()
    {
        await client.LoginAsync(new Uri("http://localhost:5000/"), new LoginModel { Username = "testar2\\testar", Password = "testar" });

        //         var modelIdentifier = "1chdi5230521708089";

        var model1 = await modelService.ByIdentifier(new ModelIdentifier("1chxaqf301488509161"));
        var model2 = await modelService.ByIdentifier(new ModelIdentifier("1chdi5230521708089"));

        var result = await graphComparer.Compare(model1, model2);

        var json1 = System.Text.Json.JsonSerializer.Serialize(result.GraphApp1);
        var json2 = System.Text.Json.JsonSerializer.Serialize(result.GraphApp2);
    }
}