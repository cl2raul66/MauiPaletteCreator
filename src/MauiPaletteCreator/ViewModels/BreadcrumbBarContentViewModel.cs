using CommunityToolkit.Mvvm.ComponentModel;
using MauiPaletteCreator.Models.View;
using MauiPaletteCreator.Services;

namespace MauiPaletteCreator.ViewModels;

public partial class BreadcrumbBarContentViewModel : ObservableRecipient
{
    public BreadcrumbBarContentViewModel(IBreadcrumbBarService breadcrumbBarService)
    {
        MainItem = breadcrumbBarService.Items[0];
        ProjectItem = breadcrumbBarService.Items[1];
        ColorsItem = breadcrumbBarService.Items[2];
        ViewItem = breadcrumbBarService.Items[3];
        EndItem = breadcrumbBarService.Items[4];
    }

    [ObservableProperty]
    BreadcrumbBarItem? mainItem;

    [ObservableProperty]
    BreadcrumbBarItem? projectItem;

    [ObservableProperty]
    BreadcrumbBarItem? colorsItem;

    [ObservableProperty]
    BreadcrumbBarItem? viewItem;

    [ObservableProperty]
    BreadcrumbBarItem? endItem;
}
