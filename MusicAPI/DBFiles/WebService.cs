using Dapper;
using MusicAPI.Dtos;

namespace MusicAPI.DBFiles; 
public partial class DBService {
    public Task<IEnumerable<WebInformation>> GetWebs() =>
        conn.WithConnectionAsync(async conn => {
            var GetVersion = "select * from web_info;";
            try {
                return await conn.QueryAsync<WebInformation>(GetVersion);
            } catch {
                return [];
            }
        }
    );
}