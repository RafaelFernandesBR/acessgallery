using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class UsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; set; }

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; set; }

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; set; }

    [JsonPropertyName("promptTokensDetails")]
    public List<TokenDetails> PromptTokensDetails { get; set; } = new();

    [JsonPropertyName("candidatesTokensDetails")]
    public List<TokenDetails> CandidatesTokensDetails { get; set; } = new();
}
