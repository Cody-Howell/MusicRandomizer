namespace MusicAPI;

public static class MusicEndpoints {
    public static WebApplication MapMusicEndpoints(this WebApplication app, string start) {
        app.MapGet(start + "/version", () => 1.0);

        return app;
    }
}
