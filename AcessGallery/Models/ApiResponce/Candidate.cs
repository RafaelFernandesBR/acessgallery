using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class Candidate
{
    [JsonPropertyName("content")]
    public Content Content { get; set; }

    [JsonPropertyName("finishReason")]
    public string FinishReason { get; set; } = string.Empty;

    [JsonPropertyName("avgLogprobs")]
    public double AvgLogprobs { get; set; }
}
