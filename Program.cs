using PersonalWebHub.Services.Rpg.Implementation;
using PersonalWebHub.Services.Rpg.Interfaces;
using System.Security.Cryptography;
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