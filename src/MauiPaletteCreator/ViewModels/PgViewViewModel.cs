﻿using CommunityToolkit.Mvvm.ComponentModel;
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

    [ObservableProperty]
    string? statusInformationText;

    [RelayCommand]
    async Task Preview()
    {
        var pgViewLbProjectStatusExecute = App.Current?.Resources["lang:PgViewLbStatusInformationTextExecute"] as string;
        if (string.IsNullOrEmpty(pgViewLbProjectStatusExecute))
        {
            return;
        }
        StatusInformationText = string.Format(pgViewLbProjectStatusExecute, SelectedPlatform);
        FileHelper.SetFilesToBeModified(testProjectServ.ProjectPath, testProjectServ.FilesToBeModified);
        await FileHelper.ApplyModificationsAsync(testProjectServ.FilesToBeModified!);
        await testProjectServ.RunProjectAsync(SelectedPlatform!);
        StatusInformationText = null;
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
