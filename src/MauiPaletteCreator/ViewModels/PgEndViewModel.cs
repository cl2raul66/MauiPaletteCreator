using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgEndViewModel : ObservableObject
{
    readonly IExternalProjectService externalProjectServ;

    public PgEndViewModel(IExternalProjectService externalProjectService)
    {
        externalProjectServ = externalProjectService;
    }

    [RelayCommand]
    async Task Apply()
    {
        if (externalProjectServ.IsLoaded)
        {
            await FileHelper.ApplyModificationsAsync(externalProjectServ.FilesToBeModified!);
        }
    }

    [RelayCommand]
    async Task GoToPgColors()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }

    #region EXTRA

    #endregion
}
