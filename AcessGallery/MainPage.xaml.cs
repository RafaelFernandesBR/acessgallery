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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.LoadPhotosCommand.CanExecute(null))
        {
            _viewModel.LoadPhotosCommand.Execute(null);
        }
    }
}
