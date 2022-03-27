using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Testar.ChangeDetection.ConsoleApp.Scenarios;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.ConsoleApp;

internal sealed partial class ConsoleHostedService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime appLifetime;
    private readonly IOptions<TestarServerOptions> orientDbOptions;
    private readonly IChangeDetectionHttpClient changeDetectionHttpClient;
    private readonly IGraphService graphService;
    private readonly IMediator mediator;
    private readonly IScenario scenario;
    private readonly CompareOptions compareOptions;
    private Task? applicationTask;
    private int? exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IOptions<CompareOptions> compareOptions,
        IOptions<TestarServerOptions> orientDbOptions,
        IChangeDetectionHttpClient changeDetectionHttpClient,
        IGraphService graphService,
        IMediator mediator,
        IScenario scenario
        )
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.orientDbOptions = orientDbOptions;
        this.changeDetectionHttpClient = changeDetectionHttpClient;
        this.graphService = graphService;
        this.mediator = mediator;
        this.scenario = scenario;
        this.compareOptions = compareOptions.Value;
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
                    await scenario.RunAsync();

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