using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Testar.ChangeDetection.Server;
using Testar.ChangeDetection.Server.OrientDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtTokenGeneratorOptions>(
    builder.Configuration.GetSection(JwtTokenGeneratorOptions.ConfigName));

builder.Services.Configure<OrientDbOptions>(
    builder.Configuration.GetSection(OrientDbOptions.ConfigName));

builder.Services.AddCors(setup =>
{
    setup.AddPolicy(name: "myCors", builder =>
    {
        builder
            .WithOrigins("https://localhost:7206")
            .AllowAnyHeader()
            .AllowAnyMethod();
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
            ValidIssuer = builder.Configuration["JwtTokenGenerator:JwtIssuer"],
            ValidAudience = builder.Configuration["JwtTokenGenerator:JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenGenerator:JwtSecurityKey"])),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("myCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();