using MusicAPI.DBFiles;

namespace MusicAPI.Endpoints;

public static class VersionEndpoints {
    public static WebApplication MapVersionEndpoint(this WebApplication app, string start) {
        app.MapGet(start + "/version", (DBService service) => service.GetVersion());

        return app;
    }
}
