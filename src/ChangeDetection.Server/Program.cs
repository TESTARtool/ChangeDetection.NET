using ChangeDetection.Server.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(config =>
{
    config.AddPolicy(name: "myCors", builder =>
    {
        builder
            .WithOrigins("https://localhost:7206")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<JwtTokenGeneratorOptions>(
    builder.Configuration.GetSection(JwtTokenGeneratorOptions.ConfigName));

builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IOrientDbSignInProvider, ConsoleAppOrientDbSignInProvider>();
builder.Services.AddOrientDb();

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
            ValidIssuer = builder.Configuration["JwtTokenGenerator:JwtIssuer"],
            ValidAudience = builder.Configuration["JwtTokenGenerator:JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenGenerator:JwtSecurityKey"])),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("myCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class ConsoleAppOrientDbSignInProvider : IOrientDbSignInProvider
{
    public ConsoleAppOrientDbSignInProvider()
    {
    }

    public Task EnhanceHttpClient(HttpClient httpClient)
    {
        return Task.CompletedTask;
    }

    public Task<string?> GetDatabaseNameAsync()
    {
        return Task.FromResult((string?)"testar");
    }
}