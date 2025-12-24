using AcessGallery.Gateways;
using AcessGallery.Models;
using AcessGallery.Models.ApiRequests;
using AcessGallery.Models.ApiResponce;
using Microsoft.Extensions.Logging;

namespace AcessGallery.Request;

public class GeminiService(IGeminiApi _geminiApi,
ILogger<GeminiService> _logger)
    : IGeminiService
{
    public async Task<GeminiResponse> GenerateDescriptionAsync(string base64Image, string mimeType, string language)
    {
        var apiKey = await SecureStorage.Default.GetAsync("GEMINI_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger?.LogError("GEMINI_API_KEY não encontrada no SecureStorage.");
            throw new Exception("Chave de API do Gemini não configurada. Por favor, configure-a nas opções do aplicativo.");
        }

        try
        {
            // Tenta o modelo normal (Flash 2.0)
            return await CaptionImage(base64Image, mimeType, language, apiKey);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao usar Gemini 2.0 Flash, tentando Lite...");
            
            // Fallback para o modelo Lite
            try 
            {
                 return await CaptionImageLite(base64Image, mimeType, language, apiKey);
            }
            catch (Exception exLite)
            {
                 _logger?.LogError(exLite, "Erro ao usar Gemini 2.0 Flash Lite (fallback).");
                 throw new Exception($"Falha ao gerar descrição: {ex.Message} -> {exLite.Message}");
            }
        }
    }

    public async Task<GeminiResponse> CaptionImage(string base64Image, string mimeType, string language, string apiKey)
    {
        try
        {
            var request = CreateGeminiRequest(base64Image, mimeType, language);
            var response = await _geminiApi.GenerateContent(request, apiKey);

            if (!response.IsSuccessStatusCode)
            {
            _logger?.LogWarning("R  esposta da Gemini API (normal) sem sucesso. Status: {StatusCode}", response.StatusCode);
                throw new Exception($"API Error: {response.StatusCode}");
            }

            return response.Content;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao chamar Gemini API (normal)");
            throw; 
        }
    }

    public async Task<GeminiResponse> CaptionImageLite(string base64Image, string mimeType, string language, string apiKey)
    {
        try
        {
            var request = CreateGeminiRequest(base64Image, mimeType, language);
            var response = await _geminiApi.GenerateContentLite(request, apiKey);

            if (!response.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Resposta da Gemini API (Lite) sem sucesso. Status: {StatusCode}", response.StatusCode);
                throw new Exception($"API Lite Error: {response.StatusCode}");
            }

            return response.Content;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao chamar Gemini API Lite");
            throw;
        }
    }

    private GeminiRequest CreateGeminiRequest(string base64Image, string mimeType, string language)
        => new GeminiRequest
        {
            Contents = new List<RequestContent>
            {
            new RequestContent
                {
                    Parts = new List<RequestPart>
                    {
                        new RequestPart
                        {
                            InlineData = new InlineData
                            {
                                MimeType = mimeType,
                                Data = base64Image
                            }
                        },
                        new RequestPart { Text = Constantes.prompt.Replace("{language}", language)}
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                MaxOutputTokens = 2048,
                ResponseMimeType = "application/json",
                ResponseSchema = new ResponseSchema
                {
                    Type = "OBJECT",
                    Properties = new Properties
                    {
                        Description = new SchemaProperty
                        {
                            Type = "STRING"
                        }
                    }
                }
            }
        };
}
