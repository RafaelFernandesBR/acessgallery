using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CheckAndRequestPermissionsAsync();

        if (_viewModel.LoadPhotosCommand.CanExecute(null))
        {
            _viewModel.LoadPhotosCommand.Execute(null);
        }
    }

    private async Task CheckAndRequestPermissionsAsync()
    {
        try
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 13)
            {
                var statusPhotos = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (statusPhotos != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.Photos>();
                }
                
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error requesting permissions: {ex.Message}");
        }
    }
}
