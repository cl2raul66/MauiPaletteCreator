using MauiPaletteCreator.Views;

namespace MauiPaletteCreator;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgProject), typeof(PgProject));
        Routing.RegisterRoute(nameof(PgColors), typeof(PgColors));
        Routing.RegisterRoute(nameof(PgView), typeof(PgView));
        Routing.RegisterRoute(nameof(PgEnd), typeof(PgEnd));
    }
}
