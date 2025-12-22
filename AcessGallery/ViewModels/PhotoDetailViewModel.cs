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
    private async Task MoreOptionsAsync()
    {
        var action = await Shell.Current.DisplayActionSheetAsync("Opções da Foto", "Cancelar", null, "Compartilhar", "Adicionar a Álbum");

        if (action == "Compartilhar")
        {
            await ShareAsync();
        }
        else if (action == "Adicionar a Álbum")
        {
            await AddToAlbumAsync();
        }
    }

    private async Task ShareAsync()
    {
        if (string.IsNullOrEmpty(PhotoPath)) return;

        try
        {
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

            await Shell.Current.DisplayAlertAsync("Erro", "Não é possível compartilhar: caminho não é um arquivo local válido.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", "Não foi possível compartilhar a foto: " + ex.Message, "OK");
        }
    }

    private async Task AddToAlbumAsync()
    {
        try
        {
            var albums = await _dbService.GetAlbumsAsync();
            if (albums == null || !albums.Any())
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Você ainda não tem álbuns criados.", "OK");
                return;
            }

            // Simple selection using ActionSheet for now
            var albumNames = albums.Select(a => a.Name).ToArray();
            var result = await Shell.Current.DisplayActionSheetAsync("Selecione o Álbum", "Cancelar", null, albumNames);

            if (result != null && result != "Cancelar")
            {
                var selectedAlbum = albums.FirstOrDefault(a => a.Name == result);
                if (selectedAlbum != null)
                {
                    if (string.IsNullOrEmpty(PhotoPath)) return;
                    
                    await _dbService.AddPhotoToAlbumAsync(selectedAlbum.Id, PhotoPath);
                    await Shell.Current.DisplayAlertAsync("Sucesso", $"Foto adicionada ao álbum '{selectedAlbum.Name}'", "OK");
                }
            }
        }
        catch (Exception ex)
        {
             await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao adicionar foto ao álbum: {ex.Message}", "OK");
        }
    }
}
