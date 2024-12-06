using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    public PgColorsViewModel()
    {
        LoadCustomPalette();
    }

    [ObservableProperty]
    bool isSelectDarkTheme;

    [ObservableProperty]
    bool isSelectAll;

    [ObservableProperty]
    bool isSelectPRINCIPAL;

    [ObservableProperty]
    bool isSelectPrimary;

    [ObservableProperty]
    bool isSelectSecondary;

    [ObservableProperty]
    bool isSelectAccent;

    [ObservableProperty]
    bool isSelectSEMANTIC;

    [ObservableProperty]
    bool isSelectError;

    [ObservableProperty]
    bool isSelectSuccess;

    [ObservableProperty]
    bool isSelectWarning;

    [ObservableProperty]
    bool isSelectNEUTRAL;

    [ObservableProperty]
    bool isSelectForeground;

    [ObservableProperty]
    bool isSelectBackground;

    [ObservableProperty]
    bool isSelectGray250;

    [ObservableProperty]
    bool isSelectGray500;

    [ObservableProperty]
    bool isSelectGray750;

    [ObservableProperty]
    ObservableCollection<ColorPaletteDefault>? defaultPalette;

    [ObservableProperty]
    ColorPaletteDefault? selectredDefaultColor;

    [RelayCommand]
    async Task Deselect()
    {
        await Shell.Current.GoToAsync(nameof(PgView), true);
    }

    [RelayCommand]
    async Task GoToNext()
    {
        await Shell.Current.GoToAsync(nameof(PgView), true);
    }

    [RelayCommand]
    async Task GoToEnd()
    {
        await Shell.Current.GoToAsync(nameof(PgEnd), true);
    }

    #region EXTRA
    partial void OnDefaultPaletteChanged(ObservableCollection<ColorPaletteDefault>? value)
    {

    }

    private void LoadCustomPalette()
    {
        HashSet<ColorPaletteDefault> allColors = [];
        var colorType = typeof(Colors);

        foreach (var field in colorType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(Color))
            {
                var color = (Color)field.GetValue(null)!;
                allColors.Add(new() { Name = field.Name, Value = color });
            }
        }

        DefaultPalette = [..allColors];
    }
    #endregion
}
