using Dapper;
using HowlDev.Web.Helpers.DbConnector;

namespace MusicAPI;

public class DBService(DbConnector conn) {
    public Task<int> GetVersion() =>
        conn.WithConnectionAsync(async conn => {
            var GetVersion = "select v from web_version";
            try {
                return await conn.QuerySingleAsync<int>(GetVersion);
            } catch {
                return 0;
            }
        }
    );

}