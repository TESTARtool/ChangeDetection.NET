using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.Strategy;

namespace Testar.ChangeDetection.ConsoleApp;

internal sealed partial class ConsoleHostedService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime appLifetime;
    private readonly IChangeDetectionStrategy strategy;
    private readonly IMediator mediator;
    private readonly IOrientDbLoginService loginService;
    private readonly IOrientDbCommandExecuter orientDbCommand;
    private readonly IGraphService graphService;
    private readonly CompareOptions compareOptions;
    private Task? applicationTask;
    private int? exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IChangeDetectionStrategy strategy,
        IMediator mediator,
        IOptions<CompareOptions> compareOptions,
        IOrientDbLoginService loginService,
        IOrientDbCommandExecuter orientDbCommand,
        IGraphService graphService
        )
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.strategy = strategy;
        this.mediator = mediator;
        this.loginService = loginService;
        this.orientDbCommand = orientDbCommand;
        this.graphService = graphService;
        this.compareOptions = compareOptions.Value;
    }

    public async Task RunAsync()
    {
        var modelId = new ModelIdentifier("ppsh1a2e1033379108");
        var elements = await graphService.FetchGraph(modelId, false);

        var json = graphService.GenerateJsonString(elements);

        File.WriteAllText("my-json.json", json);

        // var session = loginService.LoginAsync()
        //var control = await mediator.Send(new ApplicationRequest { ApplicationName = compareOptions.ControlName, ApplicationVersion = compareOptions.ControlVersion });

        //var sql = $"SELECT FROM AbstractState WHERE modelIdentifier = 'ppsh1a2e1033379108'";

        //var items = await orientDbCommand.ExecuteQueryAsync<JsonElement>(sql);

        //foreach (var item in items)
        //{
        //    foreach (var prop in item.EnumerateObject())
        //    {
        //        Console.WriteLine($"{prop.Name}:{prop.Value}");
        //    }
        //}

        //var test = await mediator.Send(new ApplicationRequest { ApplicationName = compareOptions.TestName, ApplicationVersion = compareOptions.TestVersion });
        //var fileHandler = new FileHandler(control, test);

        //await strategy.ExecuteChangeDetectionAsync(control, test, fileHandler);

        //Console.WriteLine();
        //Console.WriteLine();
        //Console.WriteLine("Change detection completed");
        //Console.WriteLine($"Results can be viewed at location: {fileHandler.RootFolder}");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Starting with arguments: {args}", string.Join(" ", Environment.GetCommandLineArgs()));
        }

        CancellationTokenSource? _cancellationTokenSource = null;

        appLifetime.ApplicationStarted.Register(() =>
        {
            logger.LogApplicationStarted();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            applicationTask = Task.Run(async () =>
            {
                try
                {
                    await RunAsync();

                    exitCode = 0;
                }
                catch (TaskCanceledException)
                {
                    // This means the application is shutting down, so just swallow this exception
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception!");
                    exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    appLifetime.StopApplication();
                }
            });
        });

        appLifetime.ApplicationStopping.Register(() =>
        {
            logger.LogDebug("Application is stopping");
            _cancellationTokenSource?.Cancel();
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Wait for the application logic to fully complete any cleanup tasks.
        // Note that this relies on the cancellation token to be properly used in the application.
        if (applicationTask != null)
        {
            await applicationTask;
        }

        logger.LogDebug("Exiting with return code: {exitCode}", exitCode);

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = exitCode.GetValueOrDefault(-1);
    }
}