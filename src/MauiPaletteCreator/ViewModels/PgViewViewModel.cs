using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;

namespace MauiPaletteCreator.ViewModels;

public partial class PgViewViewModel : ObservableObject
{
    readonly ITestProjectService testProjectServ;

    public PgViewViewModel(ITestProjectService testProjectService)
    {
        testProjectServ = testProjectService;
        Platforms = [.. testProjectServ.TargetPlatforms.Keys];
    }

    [ObservableProperty]
    ObservableCollection<string>? platforms;

    [ObservableProperty]
    string? selectedPlatform;

    [RelayCommand]
    async Task Preview()
    {
        FileHelper.SetFilesToBeModified(testProjectServ.ProjectPath, testProjectServ.FilesToBeModified);
        await FileHelper.ApplyModificationsAsync(testProjectServ.FilesToBeModified!);
        await testProjectServ.RunProjectAsync(SelectedPlatform!);
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
