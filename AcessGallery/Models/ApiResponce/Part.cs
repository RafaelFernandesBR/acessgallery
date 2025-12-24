using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiResponce;

public class Part
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
