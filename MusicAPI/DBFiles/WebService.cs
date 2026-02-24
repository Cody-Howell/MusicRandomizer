using System.Text.Json;
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

    public Task<WebNodes> GetNodesAsync(int webId) =>
        conn.WithConnectionAsync(async conn => {
            string sql = @"
                SELECT 
                    wn.id, wn.title, wn.artist, wn.audioId, wn.lengthToNext, wn.start,
                    nt.id, target_node.title as name, nt.index, nt.weight::text as weight
                FROM web_nodes wn
                LEFT JOIN node_targets nt ON wn.id = nt.nodeId
                LEFT JOIN web_nodes target_node ON nt.target = target_node.id
                WHERE wn.webId = @webId;";
            
            var nodeDictionary = new Dictionary<int, AudioNode>();

            var nodes = await conn.QueryAsync<AudioNode, NodeTargetTemp?, AudioNode>(
                sql,
                (node, target) => {
                    if (!nodeDictionary.TryGetValue(node.Id, out var currentNode)) {
                        currentNode = node;
                        currentNode.Next = [];
                        nodeDictionary.Add(currentNode.Id, currentNode);
                    }

                    if (target != null) {
                        // Convert JSON weight string to int[] and create NodeTarget
                        var nodeTarget = new NodeTarget {
                            Id = target.Id,
                            Name = target.Name,
                            Index = target.Index,
                            Weight = JsonSerializer.Deserialize<int[]>(target.Weight ?? "[]") ?? []
                        };
                        currentNode.Next = [.. currentNode.Next, nodeTarget];
                    }

                    return currentNode;
                },
                new { webId },
                splitOn: "id"
            );

            return new WebNodes { Nodes = [.. nodeDictionary.Values] };
        });

    private class NodeTargetTemp {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? Index { get; set; }
        public string Weight { get; set; } = "[]";
    }
}
