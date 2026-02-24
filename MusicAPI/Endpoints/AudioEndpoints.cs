using MusicAPI.DBFiles;
using MusicAPI.Dtos;

namespace MusicAPI.Endpoints;

public static class AudioEndpoints {
    public static WebApplication MapAudioEndpoints(this WebApplication app, string start) {
        // Audio endpoints
        app.MapPost(start + "/audio/upload", async (HttpContext context, DBService service) => {
            try {
                Console.WriteLine("=== Upload Request Started ===");
                Console.WriteLine($"Content-Type: {context.Request.ContentType}");
                Console.WriteLine($"Content-Length: {context.Request.ContentLength}");

                if (!context.Request.HasFormContentType) {
                    Console.WriteLine("ERROR: Request is not form content type");
                    return Results.BadRequest("Request must be multipart/form-data");
                }

                var form = await context.Request.ReadFormAsync();
                Console.WriteLine($"Form fields count: {form.Count}");
                Console.WriteLine($"Files count: {form.Files.Count}");

                if (!form.Files.Any()) {
                    Console.WriteLine("ERROR: No files in form");
                    return Results.BadRequest("No audio file provided");
                }

                var audioFile = form.Files["audioFile"];
                if (audioFile == null || audioFile.Length == 0) {
                    Console.WriteLine($"ERROR: Audio file is null or empty. Available files: {string.Join(", ", form.Files.Select(f => f.Name))}");
                    return Results.BadRequest("No audio file provided or file is empty");
                }

                var userGivenName = form["userGivenName"].ToString();
                var author = form["author"].ToString();

                if (string.IsNullOrEmpty(userGivenName)) {
                    Console.WriteLine("ERROR: User given name is empty");
                    return Results.BadRequest("User given name is required");
                }

                if (string.IsNullOrEmpty(author)) {
                    Console.WriteLine("ERROR: Author is empty");
                    return Results.BadRequest("Author is required");
                }

                Console.WriteLine($"Processing file: {audioFile.FileName}, Size: {audioFile.Length} bytes, Name: {userGivenName}, Author: {author}");

                using var memoryStream = new MemoryStream();
                await audioFile.CopyToAsync(memoryStream);
                var audioData = memoryStream.ToArray();

                Console.WriteLine($"File copied to memory stream, size: {audioData.Length}");

                var audioId = await service.UploadAudio(userGivenName, author, audioData);

                if (audioId == 0) {
                    Console.WriteLine("ERROR: Database insert failed");
                    return Results.Problem("Failed to upload audio file");
                }

                Console.WriteLine($"Upload successful with ID: {audioId}");
                return Results.Ok(new { Id = audioId, Message = "Audio uploaded successfully" });
            } catch (Exception ex) {
                Console.WriteLine($"EXCEPTION in upload endpoint: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Results.Problem($"Internal server error: {ex.Message}");
            }
        })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .WithMetadata(new Microsoft.AspNetCore.Mvc.RequestSizeLimitAttribute(104857600)); // 100 MB limit

        app.MapGet(start + "/audio/{id}", async (DBService service, int id) => {
            var audio = await service.GetAudioData(id);

            if (audio == null)
                return Results.NotFound("Audio not found");

            return Results.File(audio!, "audio/mpeg");
        });

        app.MapGet(start + "/audio/{id}/info", async (DBService service, int id) => {
            var audio = await service.GetAudio(id);

            if (audio == null)
                return Results.NotFound("Audio not found");

            return Results.Ok(new Audio() {
                Id = audio.Id,
                UserGivenName = audio.UserGivenName,
                Author = audio.Author
            });
        });

        app.MapGet(start + "/audio", async (DBService service) => {
            var audioList = await service.GetAllAudio();

            return Results.Ok(audioList.Select(a => new Audio() {
                Id = a.Id,
                UserGivenName = a.UserGivenName,
                Author = a.Author
            }));
        });

        return app;
    }
}
