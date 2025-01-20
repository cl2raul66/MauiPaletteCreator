using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;

namespace MauiPaletteCreator.ViewModels;

public partial class PgEndViewModel : ObservableObject
{
    readonly IExternalProjectService externalProjectServ;

    public PgEndViewModel(IExternalProjectService externalProjectService)
    {
        externalProjectServ = externalProjectService;
    }

    [ObservableProperty]
    string? statusInformationText;

    [RelayCommand]
    async Task Apply()
    {
        if (externalProjectServ.IsLoaded)
        {
            StatusInformationText = "Obteniendo los ficheros a modificar.";
            FileHelper.SetFilesToBeModified(externalProjectServ.ProjectPath, externalProjectServ.FilesToBeModified);
            StatusInformationText = "Aplicando colores y estilos.";
            await FileHelper.ApplyModificationsAsync(externalProjectServ.FilesToBeModified!);
            StatusInformationText = $"Colores y estilos aplicado al proyecto {Path.GetFileNameWithoutExtension(externalProjectServ.ProjectPath)}";
            await Task.Delay(3000);
            StatusInformationText = null;
        }
    }

    [RelayCommand]
    async Task GoToPgColors()
    {
        await Shell.Current.GoToAsync(nameof(PgColors), true);
    }

    #region EXTRA

    #endregion
}
