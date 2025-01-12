﻿using MauiPaletteCreator.Models;
using MauiPaletteCreator.Tools;
using System.Xml.Linq;

namespace MauiPaletteCreator.Services;

public interface IStyleTemplateService
{
    Task GenerateFilesToBeModifiedAsync(ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles);
    IEnumerable<ColorStyle> GetDefaultDarkColorStyle();
    IEnumerable<ColorStyle> GetDefaultLightColorStyle();
}

public class StyleTemplateService : IStyleTemplateService
{
    List<ColorStyle> PrincipalLightColorStyle = [
        new () { Name = "PrimaryCl",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light },
        new () { Name = "SecondaryCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Light },
        new() { Name = "AccentCl", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light }
    ];
    List<ColorStyle> SemanticLightColorStyle = [
        new () { Name = "ErrorCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
        new () { Name = "SuccessCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
        new() { Name = "WarningCl", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Light }
    ];
    List<ColorStyle> NeutralLightColorStyle = [
        new() { Name = "ForegroundCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "BackgroundCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "Complementary1Cl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "Complementary2Cl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "Complementary3Cl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Light }
    ];

    List<ColorStyle> PrincipalDarkColorStyle = [
        new () { Name = "PrimaryDarkCl",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark },
        new () { Name = "SecondaryDarkCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Dark },
        new() { Name = "AccentDarkCl", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark }
    ];
    List<ColorStyle> SemanticDarkColorStyle = [
        new () { Name = "ErrorDarkCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Dark },
        new () { Name = "SuccessDarkCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Dark },
        new() { Name = "WarningDarkCl", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Dark }
    ];
    List<ColorStyle> NeutralDarkColorStyle = [
        new() { Name = "ForegroundDarkCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "BackgroundDarkCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "Complementary1DarkCl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "Complementary2DarkCl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "Complementary3DarkCl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark }
    ];

    public IEnumerable<ColorStyle> GetDefaultLightColorStyle()
    {
        return PrincipalLightColorStyle.Concat(SemanticLightColorStyle).Concat(NeutralLightColorStyle);
    }

    public IEnumerable<ColorStyle> GetDefaultDarkColorStyle()
    {
        return PrincipalDarkColorStyle.Concat(SemanticDarkColorStyle).Concat(NeutralDarkColorStyle);
    }

    public async Task GenerateFilesToBeModifiedAsync(ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
    {
        var mauiFiles = new[]
        {
            Path.Combine(FileHelper.CachePath, "Colors.xaml"),
            Path.Combine(FileHelper.CachePath, "Styles.xaml")
        };
        //ModifyFileMauiColors(mauiFiles.First(), lightColorStyles, darkColorStyles);
        //ModifyFileMauiStyles(mauiFiles.Last());
        await ModifyFileMauiColorsAsync(mauiFiles.First(), lightColorStyles, darkColorStyles);
        await ModifyFileMauiStylesAsync(mauiFiles.Last());

        var androidFiles = new[]
        {
            Path.Combine(FileHelper.CachePath, "colors.xml")
        };
        //ModifyFileAndroidColors(androidFiles[0], lightColorStyles, darkColorStyles);
        await ModifyFileAndroidColorsAsync(androidFiles[0], lightColorStyles, darkColorStyles);

        //var iosFiles = new[]
        //{
        //    Path.Combine(FileHelper.CachePath,  "AppDelegate.cs"),
        //    Path.Combine(FileHelper.CachePath,  "Info.plist")
        //};

        FileHelper.ModifiedFiles["MAUI"] = [.. mauiFiles.Where(File.Exists)];
        FileHelper.ModifiedFiles["Android"] = [.. androidFiles.Where(File.Exists)];
        //FileHelper.ModifiedFiles["iOS"] = [.. iosFiles.Where(File.Exists)];
    }

    #region EXTRA
    async Task ModifyFileMauiColorsAsync(string filePath, ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
    {
        XElement? mauiColorDocument;

        var colorStyles = lightColorStyles.Concat(darkColorStyles).ToArray();

        var principalsColors = colorStyles!.Where(x => x.Tag == "PRINCIPAL");
        var semanticsColors = colorStyles!.Where(x => x.Tag == "SEMANTIC");
        var neutralsColors = colorStyles!.Where(x => x.Tag == "NEUTRAL");
        if (principalsColors is null && semanticsColors is null && neutralsColors is null)
        {
            return;
        }

        XNamespace xmlns = "http://schemas.microsoft.com/dotnet/2021/maui";
        XNamespace xmlnsx = "http://schemas.microsoft.com/winfx/2009/xaml";

        mauiColorDocument = new XElement(xmlns + "ResourceDictionary",
            new XAttribute(XNamespace.Xmlns + "x", xmlnsx.NamespaceName));

        // Principals Colors
        mauiColorDocument.Add(new XComment("PRINCIPAL"));
        foreach (var pc in principalsColors!)
        {
            mauiColorDocument.Add(new XElement(xmlns + "Color", new XAttribute(xmlnsx + "Key", pc.Name!), pc.Value!.ToArgbHex()));
        }

        // Semantics Colors
        mauiColorDocument.Add(new XComment("SEMANTIC"));
        foreach (var sc in semanticsColors!)
        {
            mauiColorDocument.Add(new XElement(xmlns + "Color", new XAttribute(xmlnsx + "Key", sc.Name!), sc.Value!.ToArgbHex()));
        }

        // Neutrals Colors
        mauiColorDocument.Add(new XComment("NEUTRAL"));
        foreach (var nc in neutralsColors!)
        {
            mauiColorDocument.Add(new XElement(xmlns + "Color", new XAttribute(xmlnsx + "Key", nc.Name!), nc.Value!.ToArgbHex()));
        }

        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        await writer.WriteLineAsync("<?xaml-comp compile=\"true\" ?>");
        await writer.WriteAsync(mauiColorDocument.ToString());
    }

    async Task ModifyFileAndroidColorsAsync(string filePath, ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
    {
        var resourcesElement = new XElement("resources");

        // Obtener los colores principales y de acento
        var colorPrimary = lightColorStyles.First(x => x.Name == "PrimaryCl")?.Value?.ToArgbHex();
        var colorPrimaryDark = darkColorStyles.First(x => x.Name == "PrimaryDarkCl")?.Value?.ToArgbHex();
        var colorAccent = lightColorStyles.First(x => x.Name == "AccentCl")?.Value?.ToArgbHex();

        // Agregar los colores principales y de acento
        resourcesElement.Add(new XElement("color", new XAttribute("name", "colorPrimary"), colorPrimary));
        resourcesElement.Add(new XElement("color", new XAttribute("name", "colorPrimaryDark"), colorPrimaryDark));
        resourcesElement.Add(new XElement("color", new XAttribute("name", "colorAccent"), colorAccent));

        var xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), resourcesElement);
        //xDocument.Save(filePath);
        using FileStream outputStream = File.Create(filePath);
        await xDocument.SaveAsync(outputStream, SaveOptions.None, CancellationToken.None);
    }

    async Task ModifyFileMauiStylesAsync(string filePath)
    {
        var text = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r<?xaml-comp compile=\"true\" ?>\r<ResourceDictionary xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">\r\r    <Style TargetType=\"ActivityIndicator\">\r        <Setter Property=\"Color\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r    </Style>\r\r    <Style TargetType=\"IndicatorView\">\r        <Setter Property=\"IndicatorColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r        <Setter Property=\"SelectedIndicatorColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r    </Style>\r\r    <Style TargetType=\"Border\">\r        <Setter Property=\"Stroke\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r        <Setter Property=\"StrokeShape\" Value=\"Rectangle\" />\r        <Setter Property=\"StrokeThickness\" Value=\"1\" />\r    </Style>\r\r    <Style TargetType=\"Stepper\">\r        <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r    </Style>\r\r    <Style TargetType=\"Button\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource BackgroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"BorderWidth\" Value=\"0\" />\r        <Setter Property=\"CornerRadius\" Value=\"8\" />\r        <Setter Property=\"Padding\" Value=\"14,10\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Pressed\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r                            <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource SecondaryCL}, Dark={StaticResource SecondaryCL}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                            <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                    <VisualState x:Name=\"PointerOver\">\r                        <VisualState.Setters>\r                            <Setter Property=\"Opacity\" Value=\"0.8\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"CheckBox\">\r        <Setter Property=\"Color\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"Color\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"DatePicker\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Editor\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource BackgroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"PlaceholderColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementarySecondCl}, Dark={StaticResource ComplementarySecondCl}}\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Entry\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource BackgroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"PlaceholderColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementarySecondCl}, Dark={StaticResource ComplementarySecondCl}}\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Frame\">\r        <Setter Property=\"HasShadow\" Value=\"False\" />\r        <Setter Property=\"Padding\" Value=\"8\" />\r        <Setter Property=\"BorderColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r        <Setter Property=\"CornerRadius\" Value=\"8\" />\r        <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource BackgroundCL}}\" />\r    </Style>\r\r    <Style TargetType=\"ImageButton\">\r        <Setter Property=\"Opacity\" Value=\"1\" />\r        <Setter Property=\"BorderColor\" Value=\"Transparent\" />\r        <Setter Property=\"BorderWidth\" Value=\"0\" />\r        <Setter Property=\"CornerRadius\" Value=\"0\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"Opacity\" Value=\"0.5\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                    <VisualState x:Name=\"PointerOver\" />\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Label\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontAutoScalingEnabled\" Value=\"False\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Span\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r    </Style>\r\r    <Style TargetType=\"ListView\">\r        <Setter Property=\"SeparatorColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r        <Setter Property=\"RefreshControlColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r    </Style>\r\r    <Style TargetType=\"Picker\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"TitleColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                            <Setter Property=\"TitleColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"ProgressBar\">\r        <Setter Property=\"ProgressColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"ProgressColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"RadioButton\">\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"RefreshView\">\r        <Setter Property=\"RefreshColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource PrimaryCL}}\" />\r    </Style>\r\r    <Style TargetType=\"SearchBar\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"PlaceholderColor\" Value=\"{StaticResource ComplementarySecondCl}\" />\r        <Setter Property=\"CancelButtonColor\" Value=\"{StaticResource PrimaryCL}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                            <Setter Property=\"PlaceholderColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"SearchHandler\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"PlaceholderColor\" Value=\"{StaticResource ComplementarySecondCl}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                            <Setter Property=\"PlaceholderColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"Shadow\">\r        <Setter Property=\"Radius\" Value=\"15\" />\r        <Setter Property=\"Opacity\" Value=\"0.5\" />\r        <Setter Property=\"Brush\" Value=\"{AppThemeBinding Light={StaticResource SecondaryCL}, Dark={StaticResource SecondaryCL}}\" />\r        <Setter Property=\"Offset\" Value=\"10,10\" />\r    </Style>\r\r    <Style TargetType=\"Slider\">\r        <Setter Property=\"MinimumTrackColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"MaximumTrackColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r        <Setter Property=\"ThumbColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"MinimumTrackColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                            <Setter Property=\"MaximumTrackColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                            <Setter Property=\"ThumbColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"SwipeItem\">\r        <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource BackgroundCL}}\" />\r    </Style>\r\r    <Style TargetType=\"Switch\">\r        <Setter Property=\"OnColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"ThumbColor\" Value=\"{StaticResource ForegroundCL}\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"OnColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                            <Setter Property=\"ThumbColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                    <VisualState x:Name=\"On\">\r                        <VisualState.Setters>\r                            <Setter Property=\"OnColor\" Value=\"{AppThemeBinding Light={StaticResource SecondaryCL}, Dark={StaticResource ComplementaryThirdCl}}\" />\r                            <Setter Property=\"ThumbColor\" Value=\"{AppThemeBinding Light={StaticResource PrimaryCL}, Dark={StaticResource ForegroundCL}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                    <VisualState x:Name=\"Off\">\r                        <VisualState.Setters>\r                            <Setter Property=\"ThumbColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryFirstCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style TargetType=\"TimePicker\">\r        <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BackgroundColor\" Value=\"Transparent\" />\r        <Setter Property=\"FontFamily\" Value=\"OpenSansRegular\"/>\r        <Setter Property=\"FontSize\" Value=\"14\" />\r        <Setter Property=\"MinimumHeightRequest\" Value=\"32\" />\r        <Setter Property=\"MinimumWidthRequest\" Value=\"32\" />\r        <Setter Property=\"VisualStateManager.VisualStateGroups\">\r            <VisualStateGroupList>\r                <VisualStateGroup x:Name=\"CommonStates\">\r                    <VisualState x:Name=\"Normal\" />\r                    <VisualState x:Name=\"Disabled\">\r                        <VisualState.Setters>\r                            <Setter Property=\"TextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryFirstCl}}\" />\r                        </VisualState.Setters>\r                    </VisualState>\r                </VisualStateGroup>\r            </VisualStateGroupList>\r        </Setter>\r    </Style>\r\r    <Style ApplyToDerivedTypes=\"True\" TargetType=\"Page\">\r        <Setter Property=\"Padding\" Value=\"0\" />\r        <Setter Property=\"BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource BackgroundCL}}\" />\r    </Style>\r\r    <Style ApplyToDerivedTypes=\"True\" TargetType=\"Shell\">\r        <Setter Property=\"Shell.BackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.ForegroundColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.TitleColor\" Value=\"{AppThemeBinding Light={StaticResource BackgroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.DisabledColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.UnselectedColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ComplementaryThirdCl}}\" />\r        <Setter Property=\"Shell.NavBarHasShadow\" Value=\"False\" />\r        <Setter Property=\"Shell.TabBarBackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource BackgroundCL}}\" />\r        <Setter Property=\"Shell.TabBarForegroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.TabBarTitleColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"Shell.TabBarUnselectedColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ComplementaryThirdCl}}\" />\r    </Style>\r\r    <Style TargetType=\"NavigationPage\">\r        <Setter Property=\"BarBackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BarTextColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"IconColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ForegroundCL}}\" />\r    </Style>\r\r    <Style TargetType=\"TabbedPage\">\r        <Setter Property=\"BarBackgroundColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"BarTextColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"UnselectedTabColor\" Value=\"{AppThemeBinding Light={StaticResource ComplementaryThirdCl}, Dark={StaticResource ForegroundCL}}\" />\r        <Setter Property=\"SelectedTabColor\" Value=\"{AppThemeBinding Light={StaticResource ForegroundCL}, Dark={StaticResource ComplementaryThirdCl}}\" />\r    </Style>\r\r</ResourceDictionary>\r\r";
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(text);
    }
    #endregion
}
