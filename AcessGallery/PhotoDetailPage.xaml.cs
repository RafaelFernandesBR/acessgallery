using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class PhotoDetailPage : ContentPage
{
	public PhotoDetailPage(PhotoDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}
