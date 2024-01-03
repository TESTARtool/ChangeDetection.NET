global using Microsoft.AspNetCore.Components.Authorization;

using BlazorApp;
using BlazorApp.Authentication;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Algorithm;
using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.ImageComparison;
using Testar.ChangeDetection.Core.Services;
using Testar.ChangeDetection.Core.Settings;
using Testar.ChangeDetection.Core.Strategy;
using Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;
using Testar.ChangeDetection.Core.Visualisation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddHttpClient();

builder.Services
    .AddScoped<IChangeDetectionHttpClient, ChangeDetectionHttpClient>();

builder.Services
    .AddScoped<IModelService, ModelService>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
    .AddAuthorizationCore()
    ;

builder.Services
    .AddScoped<IGraphService, GraphService>()
    .AddScoped<ICompareGraph, AbstractGraphCompareEngine>()
    .AddScoped<ICompareVertices, CompareVertices>()
    .AddScoped<IStartingAbstractState, InitialStartingAbstractState>()
    .AddScoped<IRetrieveGraphForComparison, GraphForCompareRetriever>()
    .AddScoped<IScreenshotService, ScreenshotService>()
    .AddScoped<IDetectChangeInCorrespondingStates, CorrespondingStatesContainsChanges>()
    ;

builder.Services
    .AddScoped<ISaveLoadSettings, BlazorSaveLoadSettings>()
    .AddScoped<ShowCompoundLayerSetting>()
    .AddScoped<ShowPrefixLabelSettings>()
    .AddScoped<AbstractStateLabelSetting>()
    .AddScoped<TestSequenceLabelSetting>()
    .AddScoped<SequenceNodeLabelSetting>()
    .AddScoped<ConcreteStateLabelSetting>()
    .AddScoped<ShowOrientDbElementData>()
    .AddScoped<MergeOverlappingEdgesSetting>()
    .AddScoped<CompareAbstractActionLabelSetting>()
    .AddScoped<ComparableDataElementNameSetting>()
    ;

builder.Services
    .AddScoped<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
    .AddScoped<IFindStateDifferences, FindStateDifferences>()
    .AddScoped<ICompareImages, SkipImageComparison>() // SkiaSharp isn't supporting BlazorApp at the moment; https://github.com/mono/SkiaSharp/issues/1219
    .AddScoped<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
    .AddScoped<IHtmlOutputter, HtmlOutputter>()
    .AddScoped<IMergeGraphFactory, MergeGraphFactory>()
    ;

builder.Services.AddLogging();
builder.Services.AddMediatR(typeof(AbstractState).Assembly);
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();