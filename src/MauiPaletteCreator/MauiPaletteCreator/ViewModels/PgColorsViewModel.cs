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
    bool isSelectAll;

    [ObservableProperty]
    bool isSelectPRINCIPAL;

    [ObservableProperty]
    bool isSelectSEMANTIC;

    [ObservableProperty]
    bool isSelectNEUTRAL;

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
        //según los check seleccionados se pondrán de ese color;
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
