using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AcessGallery.Services;
using AcessGallery.Models;

namespace AcessGallery.ViewModels;

[QueryProperty(nameof(CurrentAlbum), "CurrentAlbum")]
public partial class AlbumDetailViewModel : ObservableObject
{
    private readonly LocalDatabaseService _dbService;

    public AlbumDetailViewModel(LocalDatabaseService dbService)
    {
        _dbService = dbService;
        Photos = new ObservableCollection<PhotoItemViewModel>();
    }

    [ObservableProperty]
    private Album? _currentAlbum = null;

    [ObservableProperty]
    private ObservableCollection<PhotoItemViewModel> _photos;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _title = null;

    partial void OnCurrentAlbumChanged(Album? value)
    {
        if (value != null)
        {
            Title = value.Name;
            LoadPhotosAsync().ConfigureAwait(false);
        }
    }

    [RelayCommand]
    public async Task LoadPhotosAsync()
    {
        if (IsBusy || CurrentAlbum == null) return;
        IsBusy = true;
        try
        {
            MainThread.BeginInvokeOnMainThread(() => Photos.Clear());
            
            var albumPhotos = await _dbService.GetPhotosByAlbumIdAsync(CurrentAlbum.Id);
            
            foreach (var ap in albumPhotos)
            {
                if (string.IsNullOrEmpty(ap.FilePath)) continue;

                var desc = await _dbService.GetDescriptionAsync(ap.FilePath);
                var hasDesc = desc != null && !string.IsNullOrWhiteSpace(desc.Description);
                string hint = desc?.Description ?? AcessGallery.Helpers.PathHelper.ExtractFileName(ap.FilePath);

                var item = new PhotoItemViewModel
                {
                    FilePath = ap.FilePath,
                    HasDescription = hasDesc,
                    AccessiblityHint = hint
                };
                
                MainThread.BeginInvokeOnMainThread(() => Photos.Add(item));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    [Obsolete]
    private async Task AddPhotoAsync()
    {
        if (CurrentAlbum == null) return;

        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione uma foto",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                await _dbService.AddPhotoToAlbumAsync(CurrentAlbum.Id, result.FullPath);
                await LoadPhotosAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Não foi possível selecionar a foto: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task RenameAlbumAsync()
    {
        if (CurrentAlbum == null) return;

        string result = await Shell.Current.DisplayPromptAsync("Renomear Álbum", "Digite o novo nome do álbum:", initialValue: CurrentAlbum.Name);

        if (!string.IsNullOrWhiteSpace(result))
        {
            CurrentAlbum.Name = result.Trim();
            await _dbService.UpdateAlbumAsync(CurrentAlbum);
            Title = CurrentAlbum.Name; // Update observable property
        }
    }

    [RelayCommand]
    private async Task NavigateToPhotoDetailAsync(PhotoItemViewModel photo)
    {
        if (photo == null) return;

        var navParam = new Dictionary<string, object>
        {
            { "PhotoPath", photo.FilePath }
        };
        await Shell.Current.GoToAsync("PhotoDetailPage", navParam);
    }
}
