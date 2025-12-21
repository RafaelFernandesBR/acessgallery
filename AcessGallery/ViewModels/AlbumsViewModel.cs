using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AcessGallery.Services;
using AcessGallery.Models;

namespace AcessGallery.ViewModels;

public partial class AlbumsViewModel : ObservableObject
{
    private readonly LocalDatabaseService _dbService;

    public AlbumsViewModel(LocalDatabaseService dbService)
    {
        _dbService = dbService;
        Albums = new ObservableCollection<Album>();
    }

    [ObservableProperty]
    private ObservableCollection<Album> _albums;

    [ObservableProperty]
    private bool _isBusy;

    public async Task LoadAlbumsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Albums.Clear();
            var list = await _dbService.GetAlbumsAsync();
            foreach (var album in list)
            {
                Albums.Add(album);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateAlbumAsync()
    {
        string result = await Shell.Current.DisplayPromptAsync("Novo Álbum", "Digite o nome do álbum:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            var newAlbum = new Album { Name = result.Trim() };
            await _dbService.CreateAlbumAsync(newAlbum);
            await LoadAlbumsAsync();
        }
    }

    [RelayCommand]
    private async Task GoToAlbumDetailAsync(Album album)
    {
        if (album == null) return;
        
        var navParams = new Dictionary<string, object>
        {
            { "CurrentAlbum", album }
        };
        await Shell.Current.GoToAsync("AlbumDetailPage", navParams);
    }

    [RelayCommand]
    [Obsolete]
    private async Task DeleteAlbumAsync(Album album)
    {
        if (album == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Excluir", $"Deseja excluir o álbum '{album.Name}' e remover as fotos dele?", "Sim", "Não");
        if (confirm)
        {
            await _dbService.DeleteAlbumAsync(album);
            await LoadAlbumsAsync();
        }
    }
}
