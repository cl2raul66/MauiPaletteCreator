using CommunityToolkit.Mvvm.ComponentModel;
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
        string result = await FileHelper.LoadProjectFile();
        if (string.IsNullOrEmpty(result)) return;

        token.ThrowIfCancellationRequested();
        var isMauiApp = ProjectAnalyzerHelper.IsApplicationMaui(result);
        if (isMauiApp)
        {
            ProjectFilePath = result;
        }
    }

    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
