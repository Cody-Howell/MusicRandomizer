using MusicAPI.Dtos;

namespace MusicAPI.Endpoints;

public static class WebEndpoints {
    public static WebApplication MapWebEndpoints(this WebApplication app, string start) {
        app.MapGet(start + "/web/{id}", (DBService service, int id) => service.GetVersion());

        // Audio endpoints
        app.MapPost(start + "/audio/upload", async (DBService service, IFormFile audioFile, string userGivenName, string author) => {
            if (audioFile == null || audioFile.Length == 0)
                return Results.BadRequest("No audio file provided");
            
            if (string.IsNullOrEmpty(userGivenName))
                return Results.BadRequest("User given name is required");
            
            if (string.IsNullOrEmpty(author))
                return Results.BadRequest("Author is required");

            using var memoryStream = new MemoryStream();
            await audioFile.CopyToAsync(memoryStream);
            var audioData = memoryStream.ToArray();

            var audioId = await service.UploadAudio(userGivenName, author, audioData);
            
            if (audioId == 0)
                return Results.Problem("Failed to upload audio file");

            return Results.Ok(new { Id = audioId, Message = "Audio uploaded successfully" });
        })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data");

        app.MapGet(start + "/audio/{id}", async (DBService service, int id) => {
            var audio = await service.GetAudio(id);
            
            if (audio == null)
                return Results.NotFound("Audio not found");

            return Results.File(audio.AudioData!, "audio/mpeg", $"{audio.UserGivenName}.mp3");
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
