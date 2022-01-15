using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Testar.ChangeDetection.ConsoleApp;
using Testar.ChangeDetection.Core.OrientDb;

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ConsoleHostedService>();

        services.Configure<OrientDbOptions>(
                hostContext.Configuration.GetSection(OrientDbOptions.ConfigName));

        services.AddHttpClient();

        services.AddSingleton<IOrientDbCommand, OrientDbCommand>();
    })

    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddCommandLine(args);
        config.AddJsonFile("appsettings.json");
        config.AddEnvironmentVariables();
        config.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .RunConsoleAsync();