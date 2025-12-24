using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcessGallery.ViewModels;

public partial class AiSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _apiKey;

    public async Task InitializeAsync()
    {
        ApiKey = await SecureStorage.Default.GetAsync("GEMINI_API_KEY") ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveApiKeyAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            SecureStorage.Default.Remove("GEMINI_API_KEY");
            await Shell.Current.DisplayAlertAsync("Sucesso", "Chave de API removida.", "OK");
        }
        else
        {
            await SecureStorage.Default.SetAsync("GEMINI_API_KEY", ApiKey);
            await Shell.Current.DisplayAlertAsync("Sucesso", "Chave de API salva com sucesso.", "OK");
        }
    }
}
