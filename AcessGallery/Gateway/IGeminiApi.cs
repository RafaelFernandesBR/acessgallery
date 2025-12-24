using AcessGallery.Models.ApiRequests;
using AcessGallery.Models.ApiResponce;
using Refit;

namespace AcessGallery.Gateways;

public interface IGeminiApi
{
    [Post("/v1beta/models/gemini-2.0-flash:generateContent")]
    Task<ApiResponse<GeminiResponse>> GenerateContent(
        [Body] GeminiRequest request,
        [Query] string key);
    [Post("/v1beta/models/gemini-2.0-flash-lite:generateContent")]
    Task<ApiResponse<GeminiResponse>> GenerateContentLite(
        [Body] GeminiRequest request,
        [Query] string key);
}
