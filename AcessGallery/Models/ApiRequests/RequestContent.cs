using System.Text.Json.Serialization;

namespace AcessGallery.Models.ApiRequests;

public class RequestContent
{
    [JsonPropertyName("parts")]
    public List<RequestPart> Parts { get; set; }
}
