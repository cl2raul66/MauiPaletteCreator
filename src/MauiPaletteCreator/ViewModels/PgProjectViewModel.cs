﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgProjectViewModel : ObservableObject
{
    [ObservableProperty]
    string? projectFilePath;

    [RelayCommand(IncludeCancelCommand = true)]
    async Task LoadProject(CancellationToken token)
    {
        ProjectFilePath = null;
        string filePath = await FileHelper.LoadProjectFile();
        if (string.IsNullOrEmpty(filePath)) return;

        token.ThrowIfCancellationRequested();
        var isMauiApp = await ProjectAnalyzerHelper.IsApplicationMauiAsync(filePath);
        if (isMauiApp)
        {
            FileHelper.FilesToBeModified = await ProjectAnalyzerHelper.GetFilesToBeModifiedAsync(filePath);
            FileHelper.TargetPlatforms = await ProjectAnalyzerHelper.GetTargetPlatformsAsync(filePath);
            
            if (FileHelper.FilesToBeModified.Count > 0 && FileHelper.TargetPlatforms.Count > 0)
            {
                ProjectFilePath = filePath;
            }
        }
    }

    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
