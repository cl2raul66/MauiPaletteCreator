using MauiPaletteCreator.ViewModels;

namespace MauiPaletteCreator.Views.nav;

public partial class BreadcrumbBarContent : ContentView
{
    public BreadcrumbBarContent()
    {
        InitializeComponent();
        BindingContext = App.Current!.Handler.MauiContext!.Services.GetService<BreadcrumbBarContentViewModel>();
    }
}
