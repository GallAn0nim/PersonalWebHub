using PersonalWebHub.Services.Rpg.Implementation;
using PersonalWebHub.Services.Rpg.Interfaces;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()  
            .AllowAnyMethod()  
            .AllowAnyHeader(); 
    }));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
