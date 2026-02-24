using Dapper;
using MusicAPI.Dtos;

namespace MusicAPI.DBFiles;

public partial class DBService {
    public Task<WebInformation?> GetWeb(int id) =>
        conn.WithConnectionAsync(async conn => {
            string GetVersion = "select * from web_info where id = @id;";
            try {
                return await conn.QuerySingleAsync<WebInformation>(GetVersion, new { id });
            } catch {
                return null;
            }
        }
    );

    public Task<IEnumerable<WebInformation>> GetWebs() =>
        conn.WithConnectionAsync(async conn => {
            string GetVersion = "select * from web_info;";
            try {
                return await conn.QueryAsync<WebInformation>(GetVersion);
            } catch {
                return [];
            }
        }
    );

    public Task CreateWeb(WebInformation info) =>
        conn.WithConnectionAsync(async conn => {
            string sql = "insert into web_info (webName, info, authorRoute) values (@webName, @info, @authorRoute);";
            await conn.ExecuteAsync(sql, new {
                info.WebName,
                info.Info,
                info.AuthorRoute
            });
        }
    );
}
