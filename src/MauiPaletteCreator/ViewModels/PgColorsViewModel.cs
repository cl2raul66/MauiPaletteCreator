// Ignore Spelling: colormind Api

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPaletteCreator.Models.View;
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
    readonly IExternalProjectService externalProjectServ;
    readonly ITestProjectService testProjectServ;

    public PgColorsViewModel(IStyleTemplateService styleTemplateService, IColormindApiService colormindApiService, IExternalProjectService externalProjectService, ITestProjectService testProjectService)
    {
        styleTemplateServ = styleTemplateService;
        colormindApiServ = colormindApiService;
        LoadDefaultPalettes();
        LoadLightColorStyle();
        LoadDarkColorStyle();
        externalProjectServ = externalProjectService;
        testProjectServ = testProjectService;
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
    ObservableCollection<Color>? mauiPalette;

    [ObservableProperty]
    Color? selectedMauiColor;

    [ObservableProperty]
    ObservableCollection<ColorPalette>? fluentLightPalette;

    [ObservableProperty]
    ColorPalette? selectedFluentLightColor;

    [ObservableProperty]
    ObservableCollection<ColorPalette>? fluentDarkPalette;

    [ObservableProperty]
    ColorPalette? selectedFluentDarkColor;

    [ObservableProperty]
    string? statusInformationText;

    [RelayCommand]
    void SetSelectedMauiColor()
    {
        if (IsSelectDarkTheme)
        {
            List<ColorStyleGroup> copy = [.. DarkColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedDarkColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedDarkColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedMauiColor;
            DarkColorStyles = [.. copy];
        }
        else
        {
            List<ColorStyleGroup> copy = [.. LightColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedLightColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedLightColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedMauiColor;
            LightColorStyles = [.. copy];
        }
    }

    [RelayCommand]
    async Task GenerateByColormind()
    {
        StatusInformationText = "Generando colores, espere por favor.";
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
        StatusInformationText = null;
    }

    [RelayCommand]
    async Task SetReplicate()
    {

    }

    [RelayCommand]
    async Task GoToNext()
    {
        var testProjectPath = Path.Combine(FileHelper.CachePath, "TestGallery");
        if (!testProjectServ.IsCreated || !Directory.Exists(testProjectPath))
        {
            StatusInformationText = "Creando el proyecto TestGallery para la visualización. Puede tardar un tiempo...";
            await testProjectServ.CreateProjectAsync(testProjectPath);
        }

        if (testProjectServ.IsCreated)
        {
            StatusInformationText = "Generando ficheros modificadores...";
            await GenerateModifierFiles();
            await Shell.Current.GoToAsync(nameof(PgView), true);
        }
        StatusInformationText = null;
    }

    [RelayCommand]
    async Task GoToEnd()
    {
        StatusInformationText = "Generando ficheros modificadores...";
        await GenerateModifierFiles();
        if (externalProjectServ.IsLoaded)
        {
            await Shell.Current.GoToAsync(nameof(PgEnd), true);
        }
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
    //void LoadMauiPalette()
    //{
    //    HashSet<Color> allColors = [];
    //    var colorType = typeof(Colors);

    //    foreach (var field in colorType.GetFields(BindingFlags.Public | BindingFlags.Static))
    //    {
    //        if (field.FieldType == typeof(Color))
    //        {
    //            var color = (Color)field.GetValue(null)!;
    //            allColors.Add(color);
    //        }
    //    }

    //    MauiPalette = new ObservableCollection<Color>(allColors);
    //}

    void LoadMauiPalette()
    {
        HashSet<Color> darkColors = [];
        HashSet<Color> lightColors = [];
        HashSet<Color> normalColors = [];

        var colorType = typeof(Colors);
        foreach (var field in colorType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(Color))
            {
                var color = (Color)field.GetValue(null)!;
                var colorName = field.Name;

                if (colorName.Contains("Dark"))
                {
                    darkColors.Add(color);
                }
                else if (colorName.Contains("Light"))
                {
                    lightColors.Add(color);
                }
                else
                {
                    normalColors.Add(color);
                }
            }
        }

        Func<Color, double> getColorTemperature = color =>
        {
            color.ToHsl(out float h, out float s, out float l);
            return l;
        };

        var result = lightColors.OrderBy(getColorTemperature).Reverse()
            .Concat(darkColors.OrderBy(getColorTemperature).Reverse())
            .Concat(normalColors.OrderBy(getColorTemperature).Reverse());

        MauiPalette = new ObservableCollection<Color>(result);
    }

    async Task LoadFluentPaletteFromCsv()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("fuentuicolors.csv");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        var lightColors = new List<ColorPalette>();
        var darkColors = new List<ColorPalette>();
        var lines = contents.Split('\n').Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Trim().Split(',');
            if (columns.Length < 3) continue;

            var name = columns[0];
            var lightColor = Color.FromRgba(columns[1]);
            var darkColor = Color.FromRgba(columns[2]);

            lightColors.Add(new ColorPalette(name, lightColor, ColorScheme.Light));
            darkColors.Add(new ColorPalette(name, darkColor, ColorScheme.Dark));
        }

        FluentLightPalette = new ObservableCollection<ColorPalette>(lightColors.OrderBy(x => x.Name).Reverse());
        FluentDarkPalette = new ObservableCollection<ColorPalette>(darkColors.OrderBy(x => x.Name).Reverse());
    }


    void LoadDefaultPalettes()
    {
        LoadMauiPalette();
        _ = LoadFluentPaletteFromCsv();
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
        inputColors[0] = randomColors[0];
        inputColors[4] = randomColors[4];

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
            neutralGroup[0].Value = randomColors[4]; // Foreground
            neutralGroup[1].Value = randomColors[0]; // Background
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
            neutralGroup[0].Value = randomColors[4]; // Foreground
            neutralGroup[1].Value = randomColors[0]; // Background

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

    async Task GenerateModifierFiles()
    {
        await styleTemplateServ.GenerateFilesToBeModifiedAsync(
            LightColorStyles!.SelectMany(x => x).ToArray(),
            DarkColorStyles!.SelectMany(x => x).ToArray()
        );
    }
    #endregion
}
