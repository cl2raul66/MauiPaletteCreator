using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    public PgColorsViewModel(IStyleTemplateService styleTemplateService)
    {
        PrincipalLightColorStyle = new(styleTemplateService.GetDefaultPrincipalLightColorStyle());

        PrincipalDarkColorStyle = new(styleTemplateService.GetDefaultPrincipalDarkColorStyle());
        LoadCustomPalette();
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
    ObservableCollection<ColorStyle>? principalLightColorStyle;

    [ObservableProperty]
    ColorStyle? selectedPrincipalLightColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyle>? semanticLightColorStyle;

    [ObservableProperty]
    ColorStyle? selectedSemanticLightColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyle>? neutralLightColorStyle;

    [ObservableProperty]
    ColorStyle? selectedNeutralLightColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyle>? principalDarkColorStyle;

    [ObservableProperty]
    ColorStyle? selectedPrincipalDarkColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyle>? semanticDarkColorStyle;

    [ObservableProperty]
    ColorStyle? selectedSemanticDarkColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyle>? neutralDarkColorStyle;

    [ObservableProperty]
    ColorStyle? selectedNeutralDarkColorStyle;

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
        if (!IsSelectAll)
        {

            if (IsSelectDarkTheme)
            {

            }
        }
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
