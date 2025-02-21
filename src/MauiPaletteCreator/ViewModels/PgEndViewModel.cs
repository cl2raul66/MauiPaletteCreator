using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgEndViewModel : ObservableObject
{
    readonly IBreadcrumbBarService breadcrumbBarServ;
    readonly IExternalProjectService externalProjectServ;
    readonly ITestProjectService testProjectServ;

    public PgEndViewModel(IBreadcrumbBarService breadcrumbBarService, IExternalProjectService externalProjectService, ITestProjectService testProjectService)
    {
        breadcrumbBarServ = breadcrumbBarService;
        externalProjectServ = externalProjectService;
        testProjectServ = testProjectService;
    }

    [ObservableProperty]
    string? statusInformationText;

    [ObservableProperty]
    bool isFinished;

    [RelayCommand]
    async Task Apply()
    {
        if (externalProjectServ.IsLoaded)
        {
            StatusInformationText = App.Current?.Resources["lang:PgEndLbStatusInformationTextGetFiles"] as string;
            FileHelper.SetFilesToBeModified(externalProjectServ.ProjectPath, externalProjectServ.FilesToBeModified);
            StatusInformationText = App.Current?.Resources["lang:PgEndLbStatusInformationTextApplyColorsStyles"] as string;
            await FileHelper.ApplyModificationsAsync(externalProjectServ.FilesToBeModified!);
            var pgEndLbStatusInformationTextApplyProcessCompleted = App.Current?.Resources["lang:PgEndLbStatusInformationTextApplyColorsStyles"] as string;
            if (string.IsNullOrEmpty(pgEndLbStatusInformationTextApplyProcessCompleted))
            {
                StatusInformationText = null;
                return;
            }
            StatusInformationText = string.Format(pgEndLbStatusInformationTextApplyProcessCompleted, Path.GetFileNameWithoutExtension(externalProjectServ.ProjectPath));
            await testProjectServ.DeletedProjectAsync();
            FileHelper.CleanCache();
            IsFinished = !testProjectServ.IsCreated && string.IsNullOrEmpty(testProjectServ.ProjectPath);
            StatusInformationText = null;            
        }
    }

    [RelayCommand]
    async Task GoToPgColors()
    {
        breadcrumbBarServ.GoBack(nameof(PgColors));
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }
}
