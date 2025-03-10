using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yaoc.Core.Requests.ChromaDb;

public class UpdateRequest
{
    [JsonPropertyName("ids")]
    public List<string> Ids { get; set; } = new();
    [JsonPropertyName("embeddings")]
    public List<ReadOnlyMemory<float>> Embeddings { get; set; } = new();
    [JsonPropertyName("metadatas")]
    public Dictionary<string, object> Metadatas { get; set; } = new();
    [JsonPropertyName("documents")]
    public List<string> Documents { get; set; } = new();
    [JsonPropertyName("uris")]
    public List<string> Uris { get; set; } = new();
}
