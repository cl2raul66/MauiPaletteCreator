using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgMainViewModel : ObservableRecipient
{
    readonly IBreadcrumbBarService breadcrumbBarServ;

    public PgMainViewModel(IBreadcrumbBarService breadcrumbBarService)
    {
        breadcrumbBarServ = breadcrumbBarService;
        breadcrumbBarServ.Init();
    }

    [RelayCommand]
    async Task GoToNext()
    {
        breadcrumbBarServ.GoNext(nameof(PgProject));
        await Shell.Current.GoToAsync(nameof(PgProject), true);
    }
}
