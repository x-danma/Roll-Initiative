using Microsoft.EntityFrameworkCore;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var RollInitiativeApiAllowSpecificOrigins = "_rollInitiativeApiAllowSpecificOrigins";

builder.Services.AddDbContext<InitiativeContext>(opt => opt.UseCosmos(
    builder.Configuration.GetConnectionString("CosmosDB"),
    "Games"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: RollInitiativeApiAllowSpecificOrigins,
                      corsBuilder =>
                      {
                          corsBuilder.WithOrigins(builder.Configuration.GetValue<string>("AllowedCors"))
                          .AllowAnyHeader()
                          .AllowAnyMethod();

                      });
});
var app = builder.Build();

app.Services.CreateScope().ServiceProvider
            .GetService<InitiativeContext>().Database
            .EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(RollInitiativeApiAllowSpecificOrigins);

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/games", async (InitiativeContext db) =>
    await db.Games.ToListAsync());

app.MapGet("/games/complete", async (InitiativeContext db) =>
    await db.Games.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/games/{id}", async (int id, InitiativeContext db) =>
    await db.Games.FindAsync(id)
        is Game game
            ? Results.Ok(game)
            : Results.NotFound());

app.MapPost("/games", async (Game game, InitiativeContext db) =>
{
    while (true)
    {
        game.Id = new Random().Next(99999999);
        game.IsComplete = false;
        if(await db.Games.FindAsync(game.Id) is not Game existingGame)
            break;
    }
    db.Games.Add(game);
    await db.SaveChangesAsync();

    return Results.Created($"/games/{game.Id}", game);
});

app.MapPut("/games/{id}", async (int id, Game inputTodo, InitiativeContext db) =>
{
    var game = await db.Games.FindAsync(id);

    if (game is null) return Results.NotFound();

    game.Characters = inputTodo.Characters;
    game.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/games/{id}", async (int id, InitiativeContext db) =>
{
    if (await db.Games.FindAsync(id) is Game game)
    {
        db.Games.Remove(game);
        await db.SaveChangesAsync();
        return Results.Ok(game);
    }

    return Results.NotFound();
});

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class InitiativeContext : DbContext
{
    public InitiativeContext(DbContextOptions<InitiativeContext> options) : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
}

class Game
{
    public int Id { get; set; }
    public bool IsComplete { get; set; }
    public List<Character> Characters { get; set; } = new();

    public Game(int Id)
    {
        this.Id = Id;
    }

}

class Character
{
    public string Name { get; set; }

    public Character(string name)
    {
        Name = name;
    }

    public int RollValue { get; set; }
}
