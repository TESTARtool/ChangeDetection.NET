﻿using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Runtime.InteropServices;
using Testar.ChangeDetection.ConsoleApp;
using Testar.ChangeDetection.ConsoleApp.Scenarios;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.ImageComparison;
using Testar.ChangeDetection.Core.Services;
using Testar.ChangeDetection.Core.Strategy;
using Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

TestarLogo.Display();

Console.WriteLine($"ChangeDetection.NET");

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(AppContext.BaseDirectory))
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ConsoleHostedService>();

        services.Configure<CompareOptions>(
            hostContext.Configuration.GetSection(CompareOptions.ConfigName));

        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddSingleton<IChangeDetectionHttpClient, ConsoleToClient>();
        services.AddSingleton<IScenario, TestMultiCalls>();

        services.AddHttpClient();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddScoped<ICompareImages, SkiaSharpEasySameDimensionsPixelByPixelImageComparison>();
        }
        else
        {
            // SkiaLibrary is as of today unsupported on Non Windows platforms
            services.AddScoped<ICompareImages, NoImageComparison>();
        }

        services
            .Configure<TestarServerOptions>(hostContext.Configuration.GetSection(TestarServerOptions.ConfigName));

        services
            .AddScoped<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
            .AddScoped<IFindStateDifferences, FindStateDifferences>()
            .AddScoped<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
            .AddScoped<IHtmlOutputter, HtmlOutputter>();

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
        services.AddMediatR(typeof(TestarLogo).Assembly);
    })

    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddEnvironmentVariables();
        config.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true); // this one needs to be optional because it can be provided somewhere else
        config.AddCommandLine(args);
    })
    .RunConsoleAsync();