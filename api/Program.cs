using Api.Seed;
using Api.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class InitiativeContext : DbContext
{
    public InitiativeContext()
    {

    }

    public InitiativeContext(DbContextOptions<InitiativeContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    public static async Task CheckAndSeedDatabaseAsync( DbContextOptions<InitiativeContext> options){
        using var context = new InitiativeContext(options);
        var _ = await context.Database.EnsureDeletedAsync();

        if ( await context.Database.EnsureCreatedAsync())
        {
            context.Games.AddRange(Seed.Data);
            await context.SaveChangesAsync();
        }
    }
    
}
