using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgViewViewModel : ObservableObject
{
    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgEnd), true);
    }

    [RelayCommand]
    async Task GoToBack()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
