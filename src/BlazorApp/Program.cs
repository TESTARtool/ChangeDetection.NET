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

builder.Services
    .AddScoped<IOrientDbSignInProvider, BlazorOrientDbSessionProvider>()
    .AddOrientDb();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services
    .AddScoped<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
    .AddScoped<IFindStateDifferences, FindStateDifferences>()
    .AddScoped<ICompareImages, SkipImageComparison>() // SkiaSharp isn't supporting BlazorApp at the moment; https://github.com/mono/SkiaSharp/issues/1219
    .AddScoped<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
    .AddScoped<IHtmlOutputter, HtmlOutputter>();

builder.Services.AddMediatR(typeof(OrientDbCommand).Assembly);
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();