using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class Properties
{
    [JsonPropertyName("description")]
    public SchemaProperty Description { get; set; }
}
