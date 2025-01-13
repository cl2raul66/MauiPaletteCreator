using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgProjectViewModel : ObservableObject
{
    readonly IExternalProjectService externalProjectServ; 

    public PgProjectViewModel(IExternalProjectService externalProjectService)
    {
        externalProjectServ = externalProjectService;
    }

    [ObservableProperty]
    string? projectFilePath;

    [ObservableProperty]
    bool isLoadProject;

    [RelayCommand]
    async Task LoadProject()
    {
        IsLoadProject = true;
        ProjectFilePath = null;
        string filePath = await FileHelper.LoadProjectFile();

        await externalProjectServ.LoadProjectAsync(filePath);
        if (externalProjectServ.IsLoaded)
        {
            ProjectFilePath = filePath;
        }
        IsLoadProject = false;
    }

    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
