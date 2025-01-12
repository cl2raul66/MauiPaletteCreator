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

    [RelayCommand(IncludeCancelCommand = true)]
    async Task LoadProject(CancellationToken token)
    {
        ProjectFilePath = null;
        string filePath = await FileHelper.LoadProjectFile();

        token.ThrowIfCancellationRequested();
        await externalProjectServ.LoadProjectAsync(filePath);
        if (externalProjectServ.IsLoaded)
        {
            ProjectFilePath = filePath;
        }
    }

    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
