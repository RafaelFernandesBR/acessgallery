using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AcessGallery.Services;
using AcessGallery.Models;
using Microsoft.Maui.ApplicationModel;

namespace AcessGallery.ViewModels;

[QueryProperty(nameof(PhotoPath), "PhotoPath")]
public partial class PhotoDetailViewModel : ObservableObject
{
    private readonly LocalDatabaseService _dbService;
    
    public PhotoDetailViewModel(LocalDatabaseService dbService)
    {
        _dbService = dbService;
    }

    [ObservableProperty]
    private string _photoPath;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private ImageSource _displayImage;

    partial void OnPhotoPathChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            DisplayImage = ImageSource.FromFile(value);
            LoadDescriptionAsync(value).ConfigureAwait(false);
        }
    }

    private async Task LoadDescriptionAsync(string path)
    {
        var existing = await _dbService.GetDescriptionAsync(path);
        if (existing != null)
        {
            Description = existing.Description;
        }
        else
        {
            Description = string.Empty;
        }
    }

    [RelayCommand]
    [Obsolete]
    private async Task SaveDescriptionAsync()
    {
        if (string.IsNullOrEmpty(PhotoPath)) return;

        var photoDesc = new PhotoDescription
        {
            FilePath = PhotoPath,
            Description = Description
        };

        await _dbService.SaveDescriptionAsync(photoDesc);
        
        // Feedback acessível
        await Shell.Current.DisplayAlertAsync("Sucesso", "Descrição salva com sucesso", "OK");
        
        // Poderíamos usar SemanticScreenReader para anunciar também
        SemanticScreenReader.Announce("Descrição salva com sucesso");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    [Obsolete]
    private async Task ShareAsync()
    {
        if (string.IsNullOrEmpty(PhotoPath)) return;

        try
        {
            // Se for um caminho de arquivo local, compartilha como arquivo
            if (System.IO.File.Exists(PhotoPath))
            {
                var shareFile = new ShareFile(PhotoPath);
                var request = new ShareFileRequest
                {
                    Title = "Compartilhar foto",
                    File = shareFile
                };

                await Share.RequestAsync(request);
                return;
            }

            // Não compartilhar caminhos que não sejam arquivos locais: informar erro ao usuário
            await Shell.Current.DisplayAlertAsync("Erro", "Não é possível compartilhar: caminho não é um arquivo local válido.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível compartilhar a foto: " + ex.Message, "OK");
        }
    }
}
