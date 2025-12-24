using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class GenerationConfig
{
    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; }
    [JsonPropertyName("response_mime_type")]
    public string ResponseMimeType { get; set; }
    [JsonPropertyName("response_schema")]
    public ResponseSchema ResponseSchema { get; set; }
}
