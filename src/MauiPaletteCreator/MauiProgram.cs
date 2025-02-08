using CommunityToolkit.Maui;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.ViewModels;
using MauiPaletteCreator.Views;
using Microsoft.Extensions.Logging;

namespace MauiPaletteCreator;

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
                fonts.AddFont("Iosevka-Regular.ttf", "iosevkaRegular");
                fonts.AddFont("NFCode-Regular.ttf", "nfcodeRegular");
                fonts.AddFont("icofont.ttf", "icofont");
            });

        builder.Services.AddSingleton<IStyleTemplateService, StyleTemplateService>();
        builder.Services.AddSingleton<IColormindApiService, ColormindApiService>();
        builder.Services.AddSingleton<IExternalProjectService, ExternalProjectService>();
        builder.Services.AddSingleton<ITestProjectService, TestProjectService>();

        builder.Services.AddTransient<PgMain, PgMainViewModel>();
        builder.Services.AddTransient<PgProject, PgProjectViewModel>();
        builder.Services.AddTransient<PgColors, PgColorsViewModel>();
        builder.Services.AddTransient<PgView, PgViewViewModel>();
        builder.Services.AddTransient<PgEnd, PgEndViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
