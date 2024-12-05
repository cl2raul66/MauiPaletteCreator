using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<ColorPaletteDefault>? defaultPalette;

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
    }
    #endregion
}
