using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; } = new();

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}
