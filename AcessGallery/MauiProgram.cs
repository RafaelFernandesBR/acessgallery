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

        // Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PhotoDetailPage>();

		return builder.Build();
	}
}
