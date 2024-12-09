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
            });

        builder.Services.AddSingleton<IStyleTemplateService, StyleTemplateService>();

        builder.Services.AddTransient<PgMain, PgMainViewModel>();
        builder.Services.AddTransient<PgProyect, PgProyectViewModel>();
        builder.Services.AddTransient<PgColors, PgColorsViewModel>();
        builder.Services.AddTransient<PgView, PgViewViewModel>();
        builder.Services.AddTransient<PgEnd, PgEndViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
