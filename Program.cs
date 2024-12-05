using System.Security.Cryptography;
using PersonalWebHub.Models.AppSettings;
using PersonalWebHub.Repositories;
using PersonalWebHub.Services.AboutMe;
using PersonalWebHub.Services.Rpg;
using PersonalWebHub.Services.Tools;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    }));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<RandomNumberGenerator>(_ => RandomNumberGenerator.Create());
builder.Services.AddScoped<IDiceRollService, DiceRollService>();
builder.Services.AddScoped<IAboutMeService, AboutMeService>();
builder.Services.AddScoped<IObfuscateService, ObfuscateService>();
builder.Services.AddScoped<IAboutMeRepository, AboutMeRepository>();
builder.Services.Configure<ObfuscateSettings>(builder.Configuration.GetSection("ObfuscateSettings"));

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Personal Web Hub")
        .WithTheme(ScalarTheme.Moon)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();