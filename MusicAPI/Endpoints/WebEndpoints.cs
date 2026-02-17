using MusicAPI.DBFiles;

namespace MusicAPI.Endpoints;

public static class WebEndpoints {
    public static WebApplication MapWebEndpoints(this WebApplication app, string start) {
        app.MapGet(start + "/web/{id}", (DBService service, int id) => service.GetVersion());

        

        return app;
    }
}
