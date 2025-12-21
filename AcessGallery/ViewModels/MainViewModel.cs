using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AcessGallery.Services;
using AcessGallery.Models;

namespace AcessGallery.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMediaService _mediaService;
    private readonly LocalDatabaseService _dbService;

    public MainViewModel(IMediaService mediaService, LocalDatabaseService dbService)
    {
        _mediaService = mediaService;
        _dbService = dbService;
        Photos = new ObservableCollection<PhotoItemViewModel>();
    }

    [ObservableProperty]
    private ObservableCollection<PhotoItemViewModel> _photos;

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task LoadPhotosAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Photos.Clear();
            var paths = await _mediaService.GetImagePathsAsync();
            
            foreach (var path in paths)
            {
                var desc = await _dbService.GetDescriptionAsync(path);
                var hasDesc = desc != null && !string.IsNullOrWhiteSpace(desc.Description);
                var hint = hasDesc ? desc.Description : ExtractFileName(path);

                Photos.Add(new PhotoItemViewModel 
                { 
                    FilePath = path,
                    HasDescription = hasDesc,
                    AccessiblityHint = hint
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading photos: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(PhotoItemViewModel photo)
    {
        if (photo == null) return;
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "PhotoPath", photo.FilePath }
        };
        
        await Shell.Current.GoToAsync("PhotoDetailPage", navigationParameter);
        }

    private static string ExtractFileName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "";

        var lastSlash = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
        if (lastSlash >= 0 && lastSlash + 1 < path.Length)
            return path.Substring(lastSlash + 1);

        return path;
    }

}

public class PhotoItemViewModel
{
    public string FilePath { get; set; }
    public bool HasDescription { get; set; }
    public string AccessiblityHint { get; set; }
}
