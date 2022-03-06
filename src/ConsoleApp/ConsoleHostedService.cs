using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
    private readonly CompareOptions compareOptions;
    private Task? applicationTask;
    private int? exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IOptions<CompareOptions> compareOptions,
        IOptions<TestarServerOptions> orientDbOptions,
        IChangeDetectionHttpClient changeDetectionHttpClient,
        IGraphService graphService
        )
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.orientDbOptions = orientDbOptions;
        this.changeDetectionHttpClient = changeDetectionHttpClient;
        this.graphService = graphService;
        this.compareOptions = compareOptions.Value;
    }

    public async Task RunAsync()
    {
        await changeDetectionHttpClient.LoginAsync(orientDbOptions.Value.Url, new LoginModel
        {
            Username = orientDbOptions.Value.Username,
            Password = "testar"
        });

        var modelId1 = new ModelIdentifier("1chdi5230521708089");
        var modelId2 = new ModelIdentifier("1chxaqf301488509161");

        var elements = await graphService.FetchDiffGraph(modelId1, modelId2);

        var json = graphService.GenerateJsonString(elements);

        File.WriteAllText("my-json.json", json);
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