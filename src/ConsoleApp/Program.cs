using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Testar.ChangeDetection.ConsoleApp;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.ImageComparison;
using Testar.ChangeDetection.Core.Strategy;
using Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ConsoleHostedService>();

        services.Configure<OrientDbOptions>(
                hostContext.Configuration.GetSection(OrientDbOptions.ConfigName));

        services.Configure<CompareOptions>(
            hostContext.Configuration.GetSection(CompareOptions.ConfigName));

        services.AddHttpClient();
        services.AddSingleton<IOrientDbCommand, OrientDbCommand>();
        //services.AddSingleton<IChangeDetectionStrategy, WidgetTreeInitialStateStrategy>();

        services
            .AddSingleton<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
            .AddSingleton<IFindStateDifferences, FindStateDifferences>()
            .AddSingleton<ICompareImages, SkiaSharpEasySameDimensionsPixelByPixelImageComparison>()
            .AddSingleton<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
            .AddSingleton<IHtmlOutputter, HtmlOutputter>();

        services.AddAutoMapper(configActions =>
        {
            configActions.ReplaceMemberName("_", string.Empty);
            configActions.CreateMap<string, AbstractActionId>().ConvertUsing(x => new AbstractActionId(x));
            configActions.CreateMap<string, ModelIdentifier>().ConvertUsing(x => new ModelIdentifier(x));
            configActions.CreateMap<string, ConcreteStateId>().ConvertUsing(x => new ConcreteStateId(x));
            configActions.CreateMap<string, ConcreteActionId>().ConvertUsing(x => new ConcreteActionId(x));
            configActions.CreateMap<string, ConcreteIDCustom>().ConvertUsing(x => new ConcreteIDCustom(x));
            configActions.CreateMap<string, OrientDbId>().ConvertUsing(x => new OrientDbId(x));
            configActions.CreateMap<string, AbstractStateId>().ConvertUsing(x => new AbstractStateId(x));
            configActions.CreateMap<string, WidgetId>().ConvertUsing(x => new WidgetId(x));
        });
        services.AddMediatR(typeof(OrientDbCommand).Assembly);
    })

    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddCommandLine(args);
        config.AddJsonFile("appsettings.json");
        config.AddEnvironmentVariables();
        config.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .RunConsoleAsync();