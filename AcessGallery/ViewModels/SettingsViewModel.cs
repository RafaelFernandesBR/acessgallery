using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AcessGallery.Services;

namespace AcessGallery.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly LocalDatabaseService _dbService;

    public SettingsViewModel(LocalDatabaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task BackupDatabaseAsync()
    {
        await _dbService.BackupDatabaseAsync();
    }

    [RelayCommand]
    [Obsolete]
    private async Task RestoreBackupAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione o arquivo de backup (.db3)"
            });

            if (result != null)
            {
                if (result.FileName.EndsWith(".db3", StringComparison.OrdinalIgnoreCase))
                {
                    bool success = await _dbService.RestoreDatabaseAsync(result.FullPath);
                    if (success)
                    {
                        await Shell.Current.DisplayAlertAsync("Sucesso", "Backup restaurado com sucesso. Todos os seus dados (álbuns, descrições, etc.) foram atualizados.", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlertAsync("Erro", "Falha ao restaurar o backup.", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Arquivo Inválido", "Por favor selecione um arquivo válido (.db3).", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao selecionar arquivo: {ex.Message}", "OK");
        }
    }
}
