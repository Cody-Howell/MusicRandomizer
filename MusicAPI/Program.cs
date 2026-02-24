using HowlDev.Web.Helpers.DbConnector;
using MusicAPI.DBFiles;
using MusicAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string for database not found.");

builder.Services.AddSingleton<DBService>();
builder.Services.AddSingleton<DbConnector>();

// Configure JSON serializer to preserve PascalCase property names
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// Add CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure form options for large file uploads
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options => {
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
    options.ValueLengthLimit = 104857600; // 100 MB
    options.KeyLengthLimit = 2048;
});

// Configure Kestrel server limits
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options => {
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/health", () => "Hello");

string prefix = "/api";

app.MapVersionEndpoint(prefix);
app.MapWebEndpoints(prefix);
app.MapAudioEndpoints(prefix);

app.MapFallbackToFile("index.html");

app.Run();
