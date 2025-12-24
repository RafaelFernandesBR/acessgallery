using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcessGallery.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [RelayCommand]
    private async Task NavigateToAiSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(AiSettingsPage));
    }

    [RelayCommand]
    private async Task NavigateToBackupSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(BackupSettingsPage));
    }
}
