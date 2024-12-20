using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

public partial class PgColorsViewModel : ObservableObject
{
    readonly IColormindApiService colormindApiServ;
    readonly IStyleTemplateService styleTemplateServ;

    public PgColorsViewModel(IStyleTemplateService styleTemplateService, IColormindApiService colormindApiService)
    {
        styleTemplateServ = styleTemplateService;
        //PrincipalLightColorStyle = new(styleTemplateService.GetDefaultPrincipalLightColorStyle());
        //NeutralLightColorStyle = new(styleTemplateService.GetDefaultNeutralLightColorStyle());

        //PrincipalDarkColorStyle = new(styleTemplateService.GetDefaultPrincipalDarkColorStyle());
        LoadCustomPalette();
        //LightColorStyles = new(styleTemplateService.GetDefaultLightColorStyle());
        //DarkColorStyles = new(styleTemplateService.GetDefaultDarkColorStyle());
        LoadLightColorStyle();
        LoadDarkColorStyle();
        colormindApiServ = colormindApiService;
    }

    [ObservableProperty]
    bool isSelectDarkTheme;

    [ObservableProperty]
    bool isSelectAll = true;

    [ObservableProperty]
    bool isSelectPRINCIPAL;

    [ObservableProperty]
    bool isSelectSEMANTIC;

    [ObservableProperty]
    bool isSelectNEUTRAL;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? principalLightColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedPrincipalLightColorStyle;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? semanticLightColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedSemanticLightColorStyle;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? neutralLightColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedNeutralLightColorStyle;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? principalDarkColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedPrincipalDarkColorStyle;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? semanticDarkColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedSemanticDarkColorStyle;

    //[ObservableProperty]
    //ObservableCollection<ColorStyle>? neutralDarkColorStyle;

    //[ObservableProperty]
    //ColorStyle? selectedNeutralDarkColorStyle;


    [ObservableProperty]
    ObservableCollection<ColorStyleGroup>? lightColorStyles;

    [ObservableProperty]
    ColorStyle? selectedLightColorStyle;

    [ObservableProperty]
    ObservableCollection<ColorStyleGroup>? darkColorStyles;

    [ObservableProperty]
    ColorStyle? selectedDarkColorStyle;


    [ObservableProperty]
    ObservableCollection<Color>? defaultPalette;

    [ObservableProperty]
    Color? selectedDefaultColor;

    [RelayCommand]
    async Task Generate()
    {

        var randomColors = await colormindApiServ.GetPaletteAsync("ui");
        if (randomColors is null || randomColors.Length == 0)
        {
            await Generate();
        }
        else
        {
            var randomColors1 = await colormindApiServ.GetPaletteWithInputAsync([randomColors.First(), null, null, null, randomColors.Last()], "ui");
            //List<ColorStyle> principalLightColor = [];
            //int j = 0;
            //for (int i = 0; i < 5; i++)
            //{
            //    if (i == 0)
            //    {
            //        var NeutralLightColorStyle0 = NeutralLightColorStyle![0];
            //        NeutralLightColorStyle0.Value = randomColors[0];
            //        NeutralLightColorStyle![0] = NeutralLightColorStyle0;
            //        continue;
            //    }

            //    if (i == 4)
            //    {
            //        var NeutralLightColorStyle1 = NeutralLightColorStyle![1];
            //        NeutralLightColorStyle1.Value = randomColors[i];
            //        NeutralLightColorStyle![1] = NeutralLightColorStyle1;
            //        continue;
            //    }

            //    if (i > 0 && i < 4)
            //    {
            //        var NeutralLightColorStyle1_3 = NeutralLightColorStyle![i];
            //        NeutralLightColorStyle1_3.Value = randomColors1[i];
            //        NeutralLightColorStyle![i] = NeutralLightColorStyle1_3;
            //    }

            //    principalLightColor.Add(PrincipalLightColorStyle![j]);
            //    principalLightColor[j].Value = randomColors[i];
            //    j++;
            //}
            //PrincipalLightColorStyle = [.. principalLightColor];
        }
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
    partial void OnIsSelectDarkThemeChanged(bool value)
    {
        if (value)
        {
            SelectedLightColorStyle = null;
        }
        else
        {
            SelectedDarkColorStyle = null;
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

    void LoadLightColorStyle()
    {
        var defaultTemplate = styleTemplateServ.GetDefaultLightColorStyle();
        if (defaultTemplate is not null)
        {
            List<ColorStyleGroup> groups = [];
            foreach (var item in defaultTemplate!.Where(x => x.Scheme == ColorScheme.Light).GroupBy(cs => cs.Tag))
            {
                var s = item.ToArray();
                var group = new ColorStyleGroup(item.Key!, item.ToArray());
                groups.Add(group);
            }
            LightColorStyles = [.. groups];
        }
    }

    void LoadDarkColorStyle()
    {
        var defaultTemplate = styleTemplateServ.GetDefaultDarkColorStyle();
        List<ColorStyleGroup> groups = [];
        foreach (var item in defaultTemplate!.Where(x => x.Scheme == ColorScheme.Dark).GroupBy(cs => cs.Tag))
        {
            var s = item.ToArray();
            var group = new ColorStyleGroup(item.Key!, item.ToArray());
            groups.Add(group);
        }
        DarkColorStyles = [.. groups];
    }

    #endregion
}
