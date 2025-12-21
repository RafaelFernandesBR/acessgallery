using Microsoft.Extensions.Logging;

namespace AcessGallery;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<Services.LocalDatabaseService>();
        
#if ANDROID
        builder.Services.AddTransient<Services.IMediaService, Services.MediaService>();
#endif

        // ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.PhotoDetailViewModel>();
        builder.Services.AddTransient<ViewModels.AlbumsViewModel>();
        builder.Services.AddTransient<ViewModels.AlbumDetailViewModel>();
        builder.Services.AddTransient<ViewModels.SettingsViewModel>();

        // Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PhotoDetailPage>();
        builder.Services.AddTransient<AlbumsPage>();
        builder.Services.AddTransient<AlbumDetailPage>();
        builder.Services.AddTransient<SettingsPage>();

		return builder.Build();
	}
}
