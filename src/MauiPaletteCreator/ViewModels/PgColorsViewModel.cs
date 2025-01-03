// Ignore Spelling: colormind Api

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models;
using MauiPaletteCreator.Services;
using MauiPaletteCreator.Tools;
using MauiPaletteCreator.Views;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MauiPaletteCreator.ViewModels;

// nota: para obtener los colores semántico desde la api de ColorMind, se envía un array de 5 elementos, el segundo elemento es amarillo, el cuarto es verde y el resto null, el array de 5 elementos resultante tomamos el primer elemento para ErrorCL, el tercer para WarningCL y el quinto para SuccessCL 

public partial class PgColorsViewModel : ObservableObject
{
    readonly IColormindApiService colormindApiServ;
    readonly IStyleTemplateService styleTemplateServ;

    public PgColorsViewModel(IStyleTemplateService styleTemplateService, IColormindApiService colormindApiService)
    {
        styleTemplateServ = styleTemplateService;
        colormindApiServ = colormindApiService;
        LoadCustomPalette();
        LoadLightColorStyle();
        LoadDarkColorStyle();
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
    void SetSelectedDefaultColor()
    {
        if (IsSelectDarkTheme)
        {
            List<ColorStyleGroup> copy = [.. DarkColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedDarkColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedDarkColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedDefaultColor;
            DarkColorStyles = [.. copy];
        }
        else
        {
            List<ColorStyleGroup> copy = [.. LightColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedLightColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedLightColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedDefaultColor;
            LightColorStyles = [.. copy];
        }
    }

    [RelayCommand]
    async Task Generate()
    {
        if (IsSelectDarkTheme)
        {
            SelectedDarkColorStyle = null;
        }
        else
        {
            SelectedLightColorStyle = null;
        }

        if (IsSelectAll)
        {
            await GenerateForAll();
        }

        if (IsSelectPRINCIPAL)
        {
            await GenerateForPRINCIPAL();
        }

        if (IsSelectSEMANTIC)
        {
            await GenerateForSEMANTIC();
        }

        if (IsSelectNEUTRAL)
        {
            await GenerateForNEUTRAL();
        }
    }

    [RelayCommand]
    async Task GoToNext()
    {
        GenerateFilesToBeModified();
        await Shell.Current.GoToAsync(nameof(PgView), true);
    }

    [RelayCommand]
    async Task GoToEnd()
    {
        GenerateFilesToBeModified();
        await Shell.Current.GoToAsync(nameof(PgEnd), true);
    }

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

    #region EXTRA
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

    async Task GenerateForAll()
    {
        List<ColorStyleGroup> copyColorStyles = IsSelectDarkTheme ? [.. DarkColorStyles!] : [.. LightColorStyles!];

        var randomColors = await colormindApiServ.GetPaletteAsync("ui");

        var inputColors = new Color?[5];
        inputColors[0] = randomColors[0]; // Foreground
        inputColors[4] = randomColors[4]; // Background

        var randomColors2 = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");

        var principalGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (principalGroup is not null)
        {
            for (int i = 0; i < 3; i++)
            {
                principalGroup[i].Value = randomColors[i + 1];
            }
        }

        var neutralGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "NEUTRAL");
        if (neutralGroup is not null)
        {
            neutralGroup[0].Value = randomColors[0];
            neutralGroup[1].Value = randomColors[4];
            for (int i = 2; i < 5; i++)
            {
                neutralGroup[i].Value = randomColors2[i - 1]; // Complementary1Cl, Complementary2Cl, Complementary3Cl
            }
        }

        var semanticGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "SEMANTIC");
        for (int i = 0; i < 3; i++)
        {
            inputColors = [principalGroup![0].Value, semanticGroup![i].Value, null, null, null];
            var colors = await colormindApiServ.GetPaletteWithInputAsync(inputColors);
            semanticGroup![i].Value = colors[2];
        }

        if (IsSelectDarkTheme)
        {
            DarkColorStyles = [.. copyColorStyles];
        }
        else
        {
            LightColorStyles = [.. copyColorStyles];
        }
    }

    async Task GenerateForPRINCIPAL()
    {
        List<ColorStyleGroup> copyColorStyles = IsSelectDarkTheme ? [.. DarkColorStyles!] : [.. LightColorStyles!];
        var principalGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (principalGroup is not null)
        {
            var randomColors = await colormindApiServ.GetPaletteAsync("ui");
            for (int i = 0; i < 3; i++)
            {
                principalGroup[i].Value = randomColors[i + 1];
            }

            if (IsSelectDarkTheme)
            {
                DarkColorStyles = [.. copyColorStyles];
            }
            else
            {
                LightColorStyles = [.. copyColorStyles];
            }
        }
    }

    async Task GenerateForSEMANTIC()
    {
        List<ColorStyleGroup> copyColorStyles = IsSelectDarkTheme ? [.. DarkColorStyles!] : [.. LightColorStyles!];

        var semanticGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "SEMANTIC");
        var principalGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (semanticGroup is not null && principalGroup is not null)
        {
            for (int i = semanticGroup.Count - 1; i >= 0; i--)
            {
                var inputColors = new Color?[5];
                inputColors[0] = principalGroup[0].Value;
                inputColors[1] = semanticGroup[i].Value;
                var colors = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");
                semanticGroup[i].Value = colors[2];
            }

            if (IsSelectDarkTheme)
            {
                DarkColorStyles = [.. copyColorStyles];
            }
            else
            {
                LightColorStyles = [.. copyColorStyles];
            }
        }
    }

    async Task GenerateForNEUTRAL()
    {
        List<ColorStyleGroup> copyColorStyles = IsSelectDarkTheme ? [.. DarkColorStyles!] : [.. LightColorStyles!];

        var neutralGroup = copyColorStyles!.FirstOrDefault(g => g.Key == "NEUTRAL");
        if (neutralGroup is not null)
        {
            var randomColors = await colormindApiServ.GetPaletteAsync("ui");
            neutralGroup[0].Value = randomColors[0]; // Foreground
            neutralGroup[1].Value = randomColors[4]; // Background

            var inputColors = new Color?[5];
            inputColors[0] = randomColors[0];
            inputColors[4] = randomColors[4];
            var randomColors2 = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");

            for (int i = 2; i < 5; i++)
            {
                neutralGroup[i].Value = randomColors2[i - 1]; // Complementary1Cl, Complementary2Cl, Complementary3Cl
            }

            if (IsSelectDarkTheme)
            {
                DarkColorStyles = [.. copyColorStyles];
            }
            else
            {
                LightColorStyles = [.. copyColorStyles];
            }
        }
    }

    void GenerateFilesToBeModified()
    {
        styleTemplateServ.GenerateFilesToBeModified(
            LightColorStyles!.SelectMany(x => x).ToArray(),
            DarkColorStyles!.SelectMany(x => x).ToArray()
        );
    }
    #endregion
}
