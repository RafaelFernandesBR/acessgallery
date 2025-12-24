using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class TokenDetails
{
    [JsonPropertyName("modality")]
    public string Modality { get; set; } = string.Empty;

    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; set; }
}
