using BlazorApp;
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

builder.Services.Configure<OrientDbOptions>(
        builder.Configuration.GetSection(OrientDbOptions.ConfigName));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IOrientDbCommand, OrientDbCommand>();
//services.AddSingleton<IChangeDetectionStrategy, WidgetTreeInitialStateStrategy>();

builder.Services
    .AddSingleton<IChangeDetectionStrategy, AbstractStateComparisonStrategy>()
    .AddSingleton<IFindStateDifferences, FindStateDifferences>()
    .AddSingleton<ICompareImages, SkipImageComparison>() // SkiaSharp isn't supporting BlazorApp at the moment; https://github.com/mono/SkiaSharp/issues/1219
    .AddSingleton<IStateModelDifferenceJsonWidget, StateModelDifferenceJsonWidget>()
    .AddSingleton<IHtmlOutputter, HtmlOutputter>();

//builder.Services.AddAutoMapper(configActions =>
//{
//    configActions.ReplaceMemberName("_", string.Empty);
//    configActions.CreateMap<string, AbstractActionId>().ConvertUsing(x => new AbstractActionId(x));
//    configActions.CreateMap<string, ModelIdentifier>().ConvertUsing(x => new ModelIdentifier(x));
//    configActions.CreateMap<string, ConcreteStateId>().ConvertUsing(x => new ConcreteStateId(x));
//    configActions.CreateMap<string, ConcreteActionId>().ConvertUsing(x => new ConcreteActionId(x));
//    configActions.CreateMap<string, ConcreteIDCustom>().ConvertUsing(x => new ConcreteIDCustom(x));
//    configActions.CreateMap<string, OrientDbId>().ConvertUsing(x => new OrientDbId(x));
//    configActions.CreateMap<string, AbstractStateId>().ConvertUsing(x => new AbstractStateId(x));
//    configActions.CreateMap<string, WidgetId>().ConvertUsing(x => new WidgetId(x));
//});
builder.Services.AddMediatR(typeof(OrientDbCommand).Assembly);

await builder.Build().RunAsync();