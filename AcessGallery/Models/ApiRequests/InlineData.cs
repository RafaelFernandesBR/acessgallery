using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class InlineData
{
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = "image/jpeg";

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}
