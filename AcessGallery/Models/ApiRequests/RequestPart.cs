using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class RequestPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("inline_data")]
    public InlineData? InlineData { get; set; }
}
