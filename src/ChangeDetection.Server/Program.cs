using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddSwaggerGen();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();