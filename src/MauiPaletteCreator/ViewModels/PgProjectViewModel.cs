using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgProjectViewModel : ObservableObject
{
    readonly IBreadcrumbBarService breadcrumbBarServ;
    readonly IExternalProjectService externalProjectServ; 

    public PgProjectViewModel(IBreadcrumbBarService breadcrumbBarService, IExternalProjectService externalProjectService)
    {
        breadcrumbBarServ = breadcrumbBarService;
        externalProjectServ = externalProjectService;
    }

    [ObservableProperty]
    string? projectFilePath;

    [ObservableProperty]
    string? statusInformationText;

    [RelayCommand]
    async Task LoadProject()
    {
        StatusInformationText = App.Current?.Resources["lang:PgProjectLbProjectFilePathStatusLoad"] as string;
        string filePath = await FileHelper.LoadProjectFile();
        if (string.IsNullOrEmpty(filePath))
        {
            ProjectFilePath = null;
            StatusInformationText = null;
            return;
        }
        await externalProjectServ.LoadProjectAsync(filePath);
        if (externalProjectServ.IsLoaded)
        {
            ProjectFilePath = filePath;
        }
        StatusInformationText = null;
    }

    [RelayCommand]
    async Task GoToNext()
    {
        breadcrumbBarServ.GoNext(nameof(PgColors));
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
