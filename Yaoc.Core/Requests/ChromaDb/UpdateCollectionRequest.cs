using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yaoc.Core.Requests.ChromaDb;

public class UpdateCollectionRequest
{
    [JsonPropertyName("new_name")]
    public string Name { get; set; }
    [JsonPropertyName("new_metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}
