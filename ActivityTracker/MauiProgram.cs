using ActivityTracker.Helpers;
using ActivityTracker.Services;
using ActivityTracker.Shared.Helpers;
using ActivityTracker.Shared.Services;
using Microsoft.Extensions.Logging;
using Shiny;

namespace ActivityTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseShiny()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "Icons");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IActivityService, ActivityService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();

            builder.Services.AddSingleton<IFileSystemHelper, FileSystemHelper>();

#if ANDROID || IOS
            builder.Services.AddGps<ActivityTrackerGpsDelegate>();
#endif

            return builder.Build();
        }
    }
}
