using MusicAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapMusicEndpoints("/api");

app.Run();
