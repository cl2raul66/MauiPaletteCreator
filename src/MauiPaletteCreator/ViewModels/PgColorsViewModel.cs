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
    ObservableCollection<ColorPaletteGroup>? mauiPalette;

    [ObservableProperty]
    ColorPalette? selectedMauiColor;

    [ObservableProperty]
    ObservableCollection<ColorPaletteGroup>? fluentPalette;

    [ObservableProperty]
    ColorPalette? selectedFluentColor;

    [ObservableProperty]
    string? statusInformationText;
    
    [ObservableProperty]
    bool isBusy;

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
            element.Value = SelectedMauiColor!.Value;
            DarkColorStyles = [.. copy];
        }
        else
        {
            List<ColorStyleGroup> copy = [.. LightColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedLightColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedLightColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedMauiColor!.Value;
            LightColorStyles = [.. copy];
        }
    }

    [RelayCommand]
    void SetSelectedFluentColor()
    {
        if (IsSelectDarkTheme)
        {
            List<ColorStyleGroup> copy = [.. DarkColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedDarkColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedDarkColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedFluentColor!.Value;
            DarkColorStyles = [.. copy];
        }
        else
        {
            List<ColorStyleGroup> copy = [.. LightColorStyles!];
            var group = copy!.FirstOrDefault(x => x.Key == SelectedLightColorStyle!.Tag);
            if (group is null) return;
            var element = group.FirstOrDefault(x => x.Name == SelectedLightColorStyle!.Name);
            if (element is null) return;
            element.Value = SelectedFluentColor!.Value;
            LightColorStyles = [.. copy];
        }
    }

    [RelayCommand]
    async Task GenerateByColormind()
    {
        StatusInformationText = App.Current?.Resources["lang:PgColorsLbStatusInformationTextGenerateColor"] as string;
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
    void SetReplica()
    {
        StatusInformationText = App.Current?.Resources["lang:PgColorsLbStatusInformationTextGenerateColor"] as string;
        if (IsSelectDarkTheme)
        {
            SelectedDarkColorStyle = null;
        }
        else
        {
            SelectedLightColorStyle = null;
        }

        var sourceCollection = IsSelectDarkTheme ? LightColorStyles : DarkColorStyles;
        var targetCollection = IsSelectDarkTheme ? DarkColorStyles : LightColorStyles;

        var updatedGroups = new List<ColorStyleGroup>();

        string? searchByTag = null;

        if (IsSelectAll)
        {
            searchByTag = string.Empty;
        }

        if (IsSelectPRINCIPAL)
        {
            searchByTag = "PRINCIPAL";
        }

        if (IsSelectSEMANTIC)
        {
            searchByTag = "SEMANTIC";
        }

        if (IsSelectNEUTRAL)
        {
            searchByTag = "NEUTRAL";
        }

        if (string.IsNullOrEmpty(searchByTag))
        {
            var colorStylesCollection = sourceCollection!.SelectMany(x => x).Select(x => new ColorStyle()
            {
                Name = IsSelectDarkTheme ? x.Name + "Dark" : x.Name!.Replace("Dark", ""),
                Value = x.Value,
                Tag = x.Tag,
                Scheme = IsSelectDarkTheme ? ColorScheme.Dark : ColorScheme.Light
            });

            var tagCollection = targetCollection!.Select(x => x.Key);

            foreach (var element in tagCollection)
            {
                updatedGroups.Add(new ColorStyleGroup(element!, colorStylesCollection.Where(x => x.Tag == element)));
            }                        
        }
        else
        {
            var colorStylesCollection = sourceCollection!.SelectMany(x => x).Where(x => x.Tag == searchByTag).Select(x => new ColorStyle()
            {
                Name = IsSelectDarkTheme ? x.Name + "Dark" : x.Name!.Replace("Dark", ""),
                Value = x.Value,
                Tag = x.Tag,
                Scheme = IsSelectDarkTheme ? ColorScheme.Dark : ColorScheme.Light
            });

            foreach (var element in targetCollection!)
            {
                if (element.Key == searchByTag)
                {
                    updatedGroups.Add(new ColorStyleGroup(searchByTag, colorStylesCollection.Where(x => x.Tag == searchByTag)));
                }
                else
                {
                    updatedGroups.Add(element);
                }
            }
        }

        if (IsSelectDarkTheme)
        {
            DarkColorStyles = [.. updatedGroups];
        }
        else
        {
            LightColorStyles = [.. updatedGroups];
        }
        StatusInformationText = null;
    }

    [RelayCommand]
    async Task GoToNext()
    {
        var testProjectPath = Path.Combine(FileHelper.CachePath, "TestGallery");
        if (!testProjectServ.IsCreated || !Directory.Exists(testProjectPath))
        {
            StatusInformationText = App.Current?.Resources["lang:PgColorsLbStatusInformationTextCreateTestGalleryProject"] as string;
            await testProjectServ.CreateProjectAsync(testProjectPath);
        }

        if (testProjectServ.IsCreated)
        {
            StatusInformationText = App.Current?.Resources["lang:PgColorsLbStatusInformationTextGenerateModifierFiles"] as string;
            await GenerateModifierFiles();
            await Shell.Current.GoToAsync(nameof(PgView), true);
        }
        StatusInformationText = null;
    }

    [RelayCommand]
    async Task GoToEnd()
    {
        StatusInformationText = App.Current?.Resources["lang:PgColorsLbStatusInformationTextGenerateModifierFiles"] as string;
        await GenerateModifierFiles();
        if (externalProjectServ.IsLoaded)
        {
            await Shell.Current.GoToAsync(nameof(PgEnd), true);
        }
        StatusInformationText = null;
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
    void LoadMauiPalette()
    {
        var colorType = typeof(Colors);
        var colors = colorType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.FieldType == typeof(Color))
            .Select(x => new ColorPalette(x.Name, (Color)x.GetValue(null)!, GetColorScheme(x.Name)));

        var groups = colors
            .GroupBy(x => x.Scheme)
            .Select(x => new ColorPaletteGroup(x.Key.ToString()!, x.OrderByDescending(c => GetColorLuminosity(c)).ToArray()));

        MauiPalette = new ObservableCollection<ColorPaletteGroup>(groups);
    }

    ColorScheme GetColorScheme(string colorName)
    {
        if (colorName.Contains("Dark"))
            return ColorScheme.Dark;
        if (colorName.Contains("Light"))
            return ColorScheme.Light;
        return ColorScheme.Normal;
    }

    double GetColorLuminosity(ColorPalette color)
    {
        color.Value.ToHsl(out float h, out float s, out float l);
        return l;
    }

    async Task LoadFluentPaletteFromCsv()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("fuentuicolors.csv");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        var colors = new List<ColorPalette>();
        var lines = contents.Split('\n').Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Trim().Split(',');
            if (columns.Length < 3) continue;

            var name = columns[0];
            var lightColor = Color.FromRgba(columns[1]);
            var darkColor = Color.FromRgba(columns[2]);

            colors.Add(new ColorPalette(name, lightColor, ColorScheme.Light));
            colors.Add(new ColorPalette(name, darkColor, ColorScheme.Dark));
        }

        var groups = colors
            .OrderBy(x => x.Name)
            .GroupBy(x => x.Scheme)
            .Select(g => new ColorPaletteGroup(g.Key.ToString()!, g.ToArray()));

        FluentPalette = new ObservableCollection<ColorPaletteGroup>(groups);
    }


    async void LoadDefaultPalettes()
    {
        IsBusy = true;
        await LoadFluentPaletteFromCsv();
        await Task.Run(() => LoadMauiPalette());
        IsBusy = false;
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
