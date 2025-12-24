using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class Items
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("properties")]
    public Properties Properties { get; set; }
}
