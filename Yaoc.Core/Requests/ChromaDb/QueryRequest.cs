using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yaoc.Core.Requests.ChromaDb;

public class QueryRequest
{
    [JsonPropertyName("where")]
    public Dictionary<string, string> Where { get; set; } = new();
    [JsonPropertyName("where_documents")]
    public Dictionary<string, string> WhereDocuments { get; set; } = new();
    [JsonPropertyName("query_embeddings")]
    public List<ReadOnlyMemory<float>> Embeddings { get; set; } = new();
    [JsonPropertyName("n_results")]
    public int Results { get; set; }
    [JsonPropertyName("include")]
    public List<string> Includes { get; set; } = new();
}
