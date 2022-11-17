using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Testar.ChangeDetection.ConsoleApp;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Algorithm;
using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.Services;
using Testar.ChangeDetection.Core.Settings;
using Testar.ChangeDetection.Core.Visualisation;

var dataContainer = new DataContainer()
{
    ServerUrl = "http://localhost:5000/",
};

var builder = new ServiceCollection();

builder
    .AddHttpClient()
    .AddSingleton(dataContainer)
    .AddScoped<IChangeDetectionHttpClient, ConsoleChangeDetectionHttpClient>()
    .AddScoped<IModelService, ModelService>()
    ;

builder
    .AddScoped<ISaveLoadSettings, ConsoleSaveLoadSettings>()
    .AddScoped<ShowCompoundLayerSetting>()
    .AddScoped<ShowPrefixLabelSettings>()
    .AddScoped<AbstractStateLabelSetting>()
    .AddScoped<TestSequenceLabelSetting>()
    .AddScoped<SequenceNodeLabelSetting>()
    .AddScoped<ConcreteStateLabelSetting>()
    .AddScoped<ShowOrientDbElementData>()
    ;

builder
    .AddScoped<IGraphService, GraphService>()
    .AddScoped<ICompareGraph, AbstractGraphCompareEngine>()
    .AddScoped<ICompareVertices, CompareVertices>()
    .AddScoped<IStartingAbstractState, InitialStartingAbstractState>()
    .AddScoped<IRetrieveGraphForComparison, GraphForCompareRetriever>()
    ;

builder.AddScoped<IMergeGraphFactory, MergeGraphFactory>();

var services = builder.BuildServiceProvider();

var consoleHttpClient = services.GetRequiredService<IChangeDetectionHttpClient>();
_ = await consoleHttpClient.LoginAsync(new Uri("http://localhost:5000/"), new LoginModel
{
    Password = "testar",
    Username = @"testar2\testar"
});

var modelService = services.GetRequiredService<IModelService>();
var graphCompareService = services.GetRequiredService<ICompareGraph>();
var graphMerger = services.GetRequiredService<IMergeGraphFactory>();

var App1ModelIdentifier = "1chxaqf301488509161";
var App2ModelIdentifier = "1chdi5230521708089";

var model1 = await modelService.ByIdentifier(new ModelIdentifier(App1ModelIdentifier));
var model2 = await modelService.ByIdentifier(new ModelIdentifier(App2ModelIdentifier));
var results = await graphCompareService.CompareAsync(model2, model1);

var mergeGraph = graphMerger.Create(results);
var options = new JsonSerializerOptions
{
    WriteIndented = true,
};

var jsonOld = JsonSerializer.Serialize(results.OldGraphApp.Elements, options);
var jsonNew = JsonSerializer.Serialize(results.NewGraphApp.Elements, options);
var jsonMerge = JsonSerializer.Serialize(mergeGraph.Elements, options);

File.WriteAllText("old,json", jsonOld);
File.WriteAllText("new,json", jsonNew);
File.WriteAllText("merge,json", jsonMerge);