using AcessGallery.Models.ApiResponce;

namespace AcessGallery.Gateways;

public interface IGeminiService
{
    Task<GeminiResponse> CaptionImage(string base64Image, string mimeType, string language, string apiKey);
    Task<GeminiResponse> CaptionImageLite(string base64Image, string mimeType, string language, string apiKey);

    Task<GeminiResponse> GenerateDescriptionAsync(string base64Image, string mimeType, string language);
}
