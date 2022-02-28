using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
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

builder.Services.AddTransient<IGenerateJwTokens, JwtTokenGenerator>();

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

builder.Services.AddHttpClient<OrientDbHttpClient>();
builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

app.MapGet("/api/document/{id}", [Authorize] async ([FromQuery] string id, [FromServices] OrientDbHttpClient httpClient, ClaimsPrincipal user) =>
{
    var result = await httpClient
        .WithSession(user.Claims)
        .DocumentAsync(new OrientDbId(id), Database.StateDatabase);

    return result is not null
        ? Results.Ok(result)
        : Results.NotFound();
});

/// <summary>
/// Logins the user to the OrientDB database, returning a java web token (JWT)
/// </summary>
/// <param name="login"></param>
/// <returns>JWT</returns>
/// <remarks>
/// Sample request:
///
///     POST /api/Login
///     {
///         "username" : "orientDb user",
///         "password" : "orientDb password"
///     }
///
/// </remarks>
app.MapPost("/api/login", async ([FromBody] LoginModel login, [FromServices] OrientDbHttpClient httpClient, [FromServices] IGenerateJwTokens jwtGenerator) =>
{
    var result = await httpClient
        .WithUsernameAndPassword(login.Username, login.Password)
        .LoginAsync(Database.StateDatabase);

    return result is not null
        ? Results.Ok(jwtGenerator.GenerateToken(login, result.SessionId))
        : Results.BadRequest();
});

app.MapPost("/api/query", [Authorize] async ([FromBody] OrientDbCommand command, [FromServices] OrientDbHttpClient httpClient, ClaimsPrincipal user) =>
{
    var result = await httpClient
        .WithSession(user.Claims)
        .QueryAsync(command, Database.StateDatabase);

    return result is not null
        ? Results.Ok(result)
        : Results.BadRequest();
});

app.MapHealthChecks("/healthz");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger(options => { options.RouteTemplate = "{documentName}/swagger.json"; });
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();