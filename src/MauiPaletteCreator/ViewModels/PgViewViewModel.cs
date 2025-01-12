using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;

namespace MauiPaletteCreator.ViewModels;

public partial class PgViewViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<string>? platforms = [.. ProjectFilesHelper.TargetPlatforms.Keys];

    [ObservableProperty]
    string? selectedPlatform;

    [RelayCommand]
    async Task Preview()
    {
        string targetFramework = ProjectFilesHelper.TargetPlatforms[SelectedPlatform!];
        _ = await ProjectFilesHelper.RunTestGalleryAsync(targetFramework);
    }

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
