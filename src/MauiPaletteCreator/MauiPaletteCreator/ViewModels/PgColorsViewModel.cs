using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    public PgColorsViewModel()
    {
        LoadCustomPalette();
        LoadNullColorStyles();
    }

    [ObservableProperty]
    bool isSelectDarkTheme;

    [ObservableProperty]
    bool isSelectAll;

    [ObservableProperty]
    bool isSelectPRINCIPAL;

    [ObservableProperty]
    bool isSelectSEMANTIC;

    [ObservableProperty]
    bool isSelectNEUTRAL;

    [ObservableProperty]
    ObservableCollection<ColorStyleGroup>? lightColorStyle;

    //[ObservableProperty]
    //ColorStyle[]? selectedLightColorsStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyleGroup>? darkColorStyle;

    //[ObservableProperty]
    //ColorStyle[]? selectedDarkColorsStyle;

    [ObservableProperty]
    ObservableCollection<Color>? defaultPalette;

    [ObservableProperty]
    Color? selectredDefaultColor;

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
    partial void OnSelectredDefaultColorChanged(Color? value)
    {
        if (IsSelectDarkTheme)
        {

        }
    }

    void LoadNullColorStyles()
    {
        LightColorStyle = [
            new("PRINCIPAL", new ColorStyle[]{
                new() { Name = "PrimaryCl",Value = Color.Parse("#FF512BD4"), Scheme = ColorScheme.Light },
                new() { Name = "SecondaryCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Light },
                new() { Name = "AccentCl", Value = Color.Parse("#FF2D6FCC"), Tag="PRINCIPAL", Scheme = ColorScheme.Light }
            }),
            new("SEMANTIC", new ColorStyle[]{
                new() { Name = "ErrorCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
                new() { Name = "SuccessCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
                new() { Name = "WarningCl", Value = Color.Parse("#FFFFFF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light }
            }),
            new("NEUTRAL", new ColorStyle[]{
                new() { Name = "ForegroundCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "BackgroundCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray250Cl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray500Cl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray750Cl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Light }
            }),
        ];

        DarkColorStyle = [
            new("PRINCIPAL", new ColorStyle[]{
                new() { Name = "PrimaryDarkCl", Value = Color.Parse("#FF512BD4"), Tag="PRINCIPAL", Scheme = ColorScheme.Light },
                new() { Name = "SecondaryDarkCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Light },
                new() { Name = "AccentDarkCl", Value = Color.Parse("#FF2D6FCC"), Tag="PRINCIPAL", Scheme = ColorScheme.Light }
            }),
            new("SEMANTIC", new ColorStyle[]{
                new() { Name = "ErrorDarkCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
                new() { Name = "SuccessDarkCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
                new() { Name = "WarningDarkCl", Value = Color.Parse("#FFFFFF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light }
            }),
            new("NEUTRAL", new ColorStyle[]{
                new() { Name = "ForegroundDarkCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "BackgroundDarkCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray250DarkCl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray500DarkCl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
                new() { Name = "Gray750DarkCl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Light }
            }),
        ];
    }

    void LoadCustomPalette()
    {
        HashSet<Color> allColors = [];
        var colorType = typeof(Colors);

        foreach (var field in colorType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(Color))
            {
                var color = (Color)field.GetValue(null)!;
                allColors.Add(color);
            }
        }

        DefaultPalette = [.. allColors];
    }
    #endregion
}
