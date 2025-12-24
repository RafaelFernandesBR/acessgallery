using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class ResponseSchema
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("properties")]
    public Properties Properties { get; set; }
}
