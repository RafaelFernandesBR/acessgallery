using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; } = new();

    [JsonPropertyName("usageMetadata")]
    public UsageMetadata UsageMetadata { get; set; } = new();

    [JsonPropertyName("modelVersion")]
    public string ModelVersion { get; set; } = string.Empty;
}
