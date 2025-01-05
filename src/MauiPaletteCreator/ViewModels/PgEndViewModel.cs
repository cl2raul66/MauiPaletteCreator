using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgEndViewModel : ObservableObject
{
    [RelayCommand]
    async Task Apply()
    {
        await FileHelper.ApplyModificationsAsync();
    }

    [RelayCommand]
    async Task GoToPgColors()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }

    #region EXTRA

    #endregion
}
