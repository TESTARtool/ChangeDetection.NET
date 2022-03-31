using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Server;
using Testar.ChangeDetection.Server.JwToken;
using Testar.ChangeDetection.Server.OrientDb;

TestarLogo.Display();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddCheck<OptionsHealthCheck>(nameof(OptionsHealthCheck))
    .AddOrientDbHealthCheck();

builder.Services.Configure<GeneratorOptions>(
    builder.Configuration.GetSection(GeneratorOptions.ConfigName));

builder.Services.Configure<OrientDbOptions>(
    builder.Configuration.GetSection(OrientDbOptions.ConfigName));

var xx = builder.Configuration[$"{OrientDbOptions.ConfigName}:{nameof(OrientDbOptions.MultiDatabaseSupport)}"];

if (xx is not null && xx == bool.TrueString)
{
    builder.Services.AddTransient<IDatabaseSelector, ClaimsDatabaseSelector>();
}
else
{
    builder.Services.AddTransient<IDatabaseSelector, OptionsDatabaseSelector>();
}

var corsPolicyName = "myCors";

builder.Services.AddCors(setup =>
{
    setup.AddPolicy(name: corsPolicyName, builder =>
    {
        builder
            .WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()
            ;
    });
});

builder.Services.AddControllers();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>()
    .AddTransient<OrientDbHttpClient>();

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Testar .NET Server",
        Version = "v1",
        Description = "Proxy request and handle authorisation to the Orient DB server",
        License = new OpenApiLicense()
        {
            Name = "BSD 3-Clause License",
            Url = new Uri("https://github.com/TESTARtool/ChangeDetection.NET/blob/main/LICENSE")
        },
    });

    var xmlFilename = $"Testar.ChangeDetection.Server.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwTokenGenerator:JwtIssuer"],
            ValidAudience = builder.Configuration["JwTokenGenerator:JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwTokenGenerator:JwtSecurityKey"])),
        };
    });

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.UseSwagger(options => { options.RouteTemplate = "{documentName}/swagger.json"; });

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => options.RoutePrefix = string.Empty);
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();