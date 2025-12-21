using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class AlbumDetailPage : ContentPage
{
	private readonly AlbumDetailViewModel _viewModel;

	public AlbumDetailPage(AlbumDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPhotosAsync();
    }
}
