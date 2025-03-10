using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yaoc.Core.Requests.ChromaDb;

public class SaveCollectionRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("configuration")]
    public Dictionary<string, object> Configuration { get; set; } = new();
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
    [JsonPropertyName("get_or_create")]
    public bool CreateIfNotExists { get; set; }

}
