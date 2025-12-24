using AcessGallery.ViewModels;

namespace AcessGallery;

public partial class BackupSettingsPage : ContentPage
{
	public BackupSettingsPage(BackupSettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
