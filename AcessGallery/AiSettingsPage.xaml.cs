using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class AiSettingsPage : ContentPage
{
	public AiSettingsPage(AiSettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AiSettingsViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
