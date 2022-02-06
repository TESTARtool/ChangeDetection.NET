global using Microsoft.AspNetCore.Components.Authorization;

using BlazorApp;
using Blazored.LocalStorage;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Testar.ChangeDetection.Core.ImageComparison;
using Testar.ChangeDetection.Core.Strategy;
using Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddHttpClient();

builder.Services.AddOrientDb<BlazorOrientDbSessionProvider>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IOrientDbCommand, OrientDbCommand>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services
    .AddSingleton<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
    .AddSingleton<IFindStateDifferences, FindStateDifferences>()
    .AddSingleton<ICompareImages, SkipImageComparison>() // SkiaSharp isn't supporting BlazorApp at the moment; https://github.com/mono/SkiaSharp/issues/1219
    .AddSingleton<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
    .AddSingleton<IHtmlOutputter, HtmlOutputter>();

builder.Services.AddMediatR(typeof(OrientDbCommand).Assembly);
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();