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

        }
        else
        {
            Color[] randomColors = [];
            var lockedColorStyles = GetLockedColorStyleGroups(LightColorStyles!);
            if (lockedColorStyles.Any())
            {
                //randomColors = await colormindApiServ.GetPaletteWithInputAsync(,"ui");
            }
            else
            {
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

    IEnumerable<ColorStyleGroup> GetLockedColorStyleGroups(ObservableCollection<ColorStyleGroup>? colorStyleGroups)
    {
        if (colorStyleGroups is null) return [];

        return colorStyleGroups.Where(g => g.Any(x => x.Locked == true));
    }

    async Task GenerateForAll()
    {
        // Crear una nueva lista para almacenar los grupos modificados
        List<ColorStyleGroup> updatedLightColorStyles = [.. LightColorStyles!];

        // Primera llamada al API
        var randomColors = await colormindApiServ.GetPaletteAsync("ui");

        // Crear el array de entrada para la segunda llamada
        var inputColors = new Color?[5];
        inputColors[0] = randomColors[0]; // ForegroundCl
        inputColors[4] = randomColors[4]; // BackgroundCl

        // Segunda llamada al API
        var randomColors2 = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");

        // Asignar los colores obtenidos a los elementos correspondientes
        var principalGroup = updatedLightColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (principalGroup is not null)
        {
            for (int i = 0; i < 3; i++)
            {
                principalGroup[i].Value = randomColors[i + 1]; // Principal colors
            }
        }

        var neutralGroup = updatedLightColorStyles!.FirstOrDefault(g => g.Key == "NEUTRAL");
        if (neutralGroup is not null)
        {
            neutralGroup[0].Value = randomColors[0]; // ForegroundCl
            neutralGroup[1].Value = randomColors[4]; // BackgroundCl
            for (int i = 2; i < 5; i++)
            {
                neutralGroup[i].Value = randomColors2[i - 1]; // Gray250Cl, Gray500Cl, Gray750Cl
            }
        }

        var semanticGroup = updatedLightColorStyles!.FirstOrDefault(g => g.Key == "SEMANTIC");
        for (int i = 0; i < 3; i++)
        {
            //inputColors = [semanticGroup![i].Value, null, null, null, null];
            //var colors = await colormindApiServ.GetPaletteWithInputAsync(inputColors);
            //semanticGroup![i].Value = colors[1];

            inputColors = [principalGroup![0].Value, semanticGroup![i].Value, null, null, null];
            var colors = await colormindApiServ.GetPaletteWithInputAsync(inputColors);
            semanticGroup![i].Value = colors[2];
        }

        // Asignar la lista modificada a LightColorStyles
        LightColorStyles = [.. updatedLightColorStyles];
    }

    async Task GenerateForPRINCIPAL()
    {
        // Crear una nueva lista para almacenar los grupos modificados
        List<ColorStyleGroup> updatedLightColorStyles = [.. LightColorStyles!];
        var principalGroup = updatedLightColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (principalGroup is not null)
        {
            var randomColors = await colormindApiServ.GetPaletteAsync("ui");
            //for (int i = 0; i < principalGroup.Count; i++)
            //{
            //    principalGroup[i].Value = randomColors[i];
            //}
            for (int i = 0; i < 3; i++)
            {
                principalGroup[i].Value = randomColors[i + 1]; // Principal colors
            }

            // Asignar la lista modificada a LightColorStyles
            LightColorStyles = [.. updatedLightColorStyles];
        }
    }

    async Task GenerateForSEMANTIC()
    {
        var semanticGroup = LightColorStyles!.FirstOrDefault(g => g.Key == "SEMANTIC");
        var principalGroup = LightColorStyles!.FirstOrDefault(g => g.Key == "PRINCIPAL");
        if (semanticGroup is not null && principalGroup is not null)
        {
            for (int i = 0; i < semanticGroup.Count; i++)
            {
                var inputColors = new Color?[5];
                inputColors[0] = principalGroup[0].Value;
                inputColors[1] = semanticGroup[i].Value;
                var colors = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");
                semanticGroup[i].Value = colors[2];
            }
        }
    }

    async Task GenerateForNEUTRAL()
    {
        var neutralGroup = LightColorStyles!.FirstOrDefault(g => g.Key == "NEUTRAL");
        if (neutralGroup is not null)
        {
            var randomColors = await colormindApiServ.GetPaletteAsync("ui");
            neutralGroup[0].Value = randomColors[0]; // ForegroundCl
            neutralGroup[1].Value = randomColors[4]; // BackgroundCl

            var inputColors = new Color?[5];
            inputColors[0] = randomColors[0]; // ForegroundCl
            inputColors[4] = randomColors[4]; // BackgroundCl
            var randomColors2 = await colormindApiServ.GetPaletteWithInputAsync(inputColors, "ui");

            for (int i = 2; i < 5; i++)
            {
                neutralGroup[i].Value = randomColors2[i - 1]; // Gray250Cl, Gray500Cl, Gray750Cl
            }
        }
    }
    #endregion
}
