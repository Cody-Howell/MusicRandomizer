using Dapper;
using HowlDev.Web.Helpers.DbConnector;
using MusicAPI.Dtos;

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

    public Task<int> UploadAudio(string userGivenName, string author, byte[] audioData) =>
        conn.WithConnectionAsync(async conn => {
            var insertAudio = "INSERT INTO audio (userGivenName, author, audio) VALUES (@UserGivenName, @Author, @AudioData) RETURNING id";
            try {
                return await conn.QuerySingleAsync<int>(insertAudio, new { 
                    UserGivenName = userGivenName, 
                    Author = author, 
                    AudioData = audioData 
                });
            } catch {
                return 0;
            }
        }
    );

    public Task<Audio?> GetAudio(int id) =>
        conn.WithConnectionAsync(async conn => {
            var getAudio = "SELECT id as Id, userGivenName as UserGivenName, author as Author, audio as AudioData FROM audio WHERE id = @Id";
            try {
                return await conn.QuerySingleOrDefaultAsync<Audio>(getAudio, new { Id = id });
            } catch {
                return null;
            }
        }
    );

    public Task<byte[]?> GetAudioData(int id) =>
        conn.WithConnectionAsync(async conn => {
            var getAudio = "SELECT audio as AudioData FROM audio WHERE id = @Id";
            try {
                return await conn.QuerySingleOrDefaultAsync<byte[]>(getAudio, new { Id = id });
            } catch {
                return null;
            }
        }
    );

    public Task<IEnumerable<Audio>> GetAllAudio() =>
        conn.WithConnectionAsync(async conn => {
            var getAllAudio = "SELECT id as Id, userGivenName as UserGivenName, author as Author, audio as AudioData FROM audio";
            try {
                return await conn.QueryAsync<Audio>(getAllAudio);
            } catch {
                return Enumerable.Empty<Audio>();
            }
        }
    );

}