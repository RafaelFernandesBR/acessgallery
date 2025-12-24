using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<RequestContent> Contents { get; set; }
    [JsonPropertyName("generationConfig")]
    public GenerationConfig GenerationConfig { get; set; }
}
