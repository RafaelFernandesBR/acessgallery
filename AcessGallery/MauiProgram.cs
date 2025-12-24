using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AcessGallery.Gateways;
using Refit;
using Polly;
using Polly.Extensions.Http;
using AcessGallery.Request;

namespace AcessGallery;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

            // Registra o IGeminiApi usando Refit  
            builder.Services.AddRefitClient<IGeminiApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
            })
            .AddPolicyHandler(Policy.WrapAsync(
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(1)), // Retry
                Policy.TimeoutAsync<HttpResponseMessage>(10) // Timeout de 10 segundos
            ));

        // Services
        builder.Services.AddSingleton<Services.LocalDatabaseService>();
            builder.Services.AddSingleton<IGeminiService, GeminiService>();
        
#if ANDROID
        builder.Services.AddTransient<Services.IMediaService, Services.MediaService>();
#endif

        // ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.PhotoDetailViewModel>();
        builder.Services.AddTransient<ViewModels.AlbumsViewModel>();
        builder.Services.AddTransient<ViewModels.AlbumDetailViewModel>();
        builder.Services.AddTransient<ViewModels.SettingsViewModel>();
        builder.Services.AddTransient<ViewModels.AiSettingsViewModel>();
        builder.Services.AddTransient<ViewModels.BackupSettingsViewModel>();

        // Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PhotoDetailPage>();
        builder.Services.AddTransient<AlbumsPage>();
        builder.Services.AddTransient<AlbumDetailPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<AiSettingsPage>();
        builder.Services.AddTransient<BackupSettingsPage>();

		return builder.Build();
	}
}
