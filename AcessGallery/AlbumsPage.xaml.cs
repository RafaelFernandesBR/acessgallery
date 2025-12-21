using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class AlbumsPage : ContentPage
{
	private readonly AlbumsViewModel _viewModel;

	public AlbumsPage(AlbumsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAlbumsAsync();
    }
}
