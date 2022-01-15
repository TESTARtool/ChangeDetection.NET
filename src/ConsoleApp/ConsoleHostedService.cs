using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.ConsoleApp;

internal sealed class ConsoleHostedService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime appLifetime;
    private readonly IOrientDbCommand orientDbCommand;
    private readonly OrientDbOptions orientDbOptions;
    private Task? applicationTask;
    private int? exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IOrientDbCommand orientDbCommand
        )
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.orientDbCommand = orientDbCommand;
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
                    Console.WriteLine("Hello World!");

                    //   var query = await orientDbCommand.ExecuteQuery("SELECT FROM AbstractStateModel");
                    await Task.Delay(1000);

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