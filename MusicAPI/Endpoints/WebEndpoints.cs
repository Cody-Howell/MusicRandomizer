using MusicAPI.DBFiles;
using MusicAPI.Dtos;

namespace MusicAPI.Endpoints;

public static class WebEndpoints {
    public static WebApplication MapWebEndpoints(this WebApplication app, string start) {
        app.MapGet(start + "/web", (DBService service) => service.GetWebs());
        // app.MapGet(start + "/web/{id}", (DBService service, int id) => service.GetWebs());

        app.MapPost(start + "/web", (DBService service, WebInformation info) => service.CreateWeb(info));

        return app;
    }
}
