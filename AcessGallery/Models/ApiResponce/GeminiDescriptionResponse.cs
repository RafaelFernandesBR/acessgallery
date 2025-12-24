using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class GeminiDescriptionResponse
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
