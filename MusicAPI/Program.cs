using HowlDev.Web.Helpers.DbConnector;
using MusicAPI;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string for database not found.");

builder.Services.AddSingleton<DBService>();
builder.Services.AddSingleton<DbConnector>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/health", () => "Hello");

app.MapMusicEndpoints("/api");

app.MapFallbackToFile("index.html");

app.Run();
