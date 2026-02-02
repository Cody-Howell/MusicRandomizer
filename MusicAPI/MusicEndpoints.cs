namespace MusicAPI;

public static class MusicEndpoints {
    public static WebApplication MapMusicEndpoints(this WebApplication app, string start) {
        app.MapGet(start + "/version", (DBService service) => service.GetVersion());

        return app;
    }
}
