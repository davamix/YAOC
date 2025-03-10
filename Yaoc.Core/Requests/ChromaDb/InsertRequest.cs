using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yaoc.Core.Requests.ChromaDb;

public class InsertRequest
{
    [JsonPropertyName("ids")]
    public List<string> Ids { get; set; } = new();
    [JsonPropertyName("embeddings")]
    public List<ReadOnlyMemory<float>> Embeddings { get; set; } = new();
    [JsonPropertyName("metadatas")]
    public List<Dictionary<string, object>> Metadata { get; set; } = new();
}
