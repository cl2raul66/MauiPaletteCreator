using MauiPaletteCreator.Models;
using MauiPaletteCreator.Tools;
using System.Xml.Linq;

namespace MauiPaletteCreator.Services;

public interface IStyleTemplateService
{
    void GenerateFilesToBeModified(ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles);
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
        new() { Name = "ForegroundCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "BackgroundCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
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
        new() { Name = "ForegroundDarkCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "BackgroundDarkCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
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

    public void GenerateFilesToBeModified(ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
    {
        var mauiFiles = new[]
        {
            Path.Combine(FileHelper.CachePath, "Colors.xaml"),
            Path.Combine(FileHelper.CachePath, "Styles.xaml")
        };
        ModifyFileMauiColors(mauiFiles.First(), lightColorStyles, darkColorStyles);
        ModifyFileMauiStyles(mauiFiles.Last(), lightColorStyles, darkColorStyles);

        var androidFiles = new[]
        {
            Path.Combine(FileHelper.CachePath, "colors.xml")
        };
        ModifyFileAndroidColors(androidFiles[0], lightColorStyles, darkColorStyles);

        //var iosFiles = new[]
        //{
        //    Path.Combine(FileHelper.CachePath,  "AppDelegate.cs"),
        //    Path.Combine(FileHelper.CachePath,  "Info.plist")
        //};

        FileHelper.FilesToBeModified["MAUI"] = [.. mauiFiles.Where(File.Exists)];
        FileHelper.FilesToBeModified["Android"] = [.. androidFiles.Where(File.Exists)];
        //FileHelper.FilesToBeModified["iOS"] = [.. iosFiles.Where(File.Exists)];
    }

    #region EXTRA
    void ModifyFileMauiColors(string filePath, ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
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

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            writer.WriteLine("<?xaml-comp compile=\"true\" ?>");
            writer.Write(mauiColorDocument.ToString());
        }
    }

    void ModifyFileAndroidColors(string filePath, ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
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
        xDocument.Save(filePath);
    }

    void ModifyFileMauiStyles(string filePath, ColorStyle[] lightColorStyles, ColorStyle[] darkColorStyles)
    {
        XElement? mauiStylesDocument;

        XNamespace xmlns = "http://schemas.microsoft.com/dotnet/2021/maui";
        XNamespace xmlnsx = "http://schemas.microsoft.com/winfx/2009/xaml";

        mauiStylesDocument = new XElement(xmlns + "ResourceDictionary",
            new XAttribute(XNamespace.Xmlns + "x", xmlnsx.NamespaceName));

        // ActivityIndicator
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "ActivityIndicator"),
            new XElement("Setter", new XAttribute("Property", "Color"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}"))
        ));

        // IndicatorView
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "IndicatorView"),
            new XElement("Setter", new XAttribute("Property", "IndicatorColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "SelectedIndicatorColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        // Border
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Border"),
            new XElement("Setter", new XAttribute("Property", "Stroke"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "StrokeShape"), new XAttribute("Value", "Rectangle")),
            new XElement("Setter", new XAttribute("Property", "StrokeThickness"), new XAttribute("Value", "1"))
        ));

        // Stepper
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Stepper"),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        // Button
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Button"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource BackgroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "BorderWidth"), new XAttribute("Value", "0")),
            new XElement("Setter", new XAttribute("Property", "CornerRadius"), new XAttribute("Value", "8")),
            new XElement("Setter", new XAttribute("Property", "Padding"), new XAttribute("Value", "14,10")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Pressed"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource BackgroundCl}}")),
                                new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}"))
                            )
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "PointerOver"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "Opacity"), new XAttribute("Value", "0.8"))
                            )
                        )
                    )
                )
            )
        ));

        // CheckBox
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "CheckBox"),
            new XElement("Setter", new XAttribute("Property", "Color"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "Color"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // DatePicker
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "DatePicker"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Editor
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Editor"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary2Cl}, Dark={StaticResource Complementary2Cl}}")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary2Cl}, Dark={StaticResource Complementary2Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Entry
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Entry"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary2Cl}, Dark={StaticResource Complementary2Cl}}")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary2Cl}, Dark={StaticResource Complementary2Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Frame
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Frame"),
            new XElement("Setter", new XAttribute("Property", "HasShadow"), new XAttribute("Value", "False")),
            new XElement("Setter", new XAttribute("Property", "Padding"), new XAttribute("Value", "8")),
            new XElement("Setter", new XAttribute("Property", "BorderColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "CornerRadius"), new XAttribute("Value", "8")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource BackgroundCl}}"))
        ));

        // ImageButton
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "ImageButton"),
            new XElement("Setter", new XAttribute("Property", "Opacity"), new XAttribute("Value", "1")),
            new XElement("Setter", new XAttribute("Property", "BorderColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "BorderWidth"), new XAttribute("Value", "0")),
            new XElement("Setter", new XAttribute("Property", "CornerRadius"), new XAttribute("Value", "0")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "Opacity"), new XAttribute("Value", "0.5"))
                            )
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "PointerOver")
                        )
                    )
                )
            )
        ));

        // Label
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Label"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontAutoScalingEnabled"), new XAttribute("Value", "False")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Span
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Span"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}"))
        ));

        // ListView
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "ListView"),
            new XElement("Setter", new XAttribute("Property", "SeparatorColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "RefreshControlColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        // Picker
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Picker"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "TitleColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "TitleColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // ProgressBar
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "ProgressBar"),
            new XElement("Setter", new XAttribute("Property", "ProgressColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "ProgressColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // RadioButton
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "RadioButton"),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // RefreshView
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "RefreshView"),
            new XElement("Setter", new XAttribute("Property", "RefreshColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        // SearchBar
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "SearchBar"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{StaticResource Complementary2Cl}")),
            new XElement("Setter", new XAttribute("Property", "CancelButtonColor"), new XAttribute("Value", "{StaticResource Complementary2Cl}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // SearchHandler
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "SearchHandler"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{StaticResource Complementary2Cl}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "PlaceholderColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Shadow
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Shadow"),
            new XElement("Setter", new XAttribute("Property", "Radius"), new XAttribute("Value", "15")),
            new XElement("Setter", new XAttribute("Property", "Opacity"), new XAttribute("Value", "0.5")),
            new XElement("Setter", new XAttribute("Property", "Brush"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Offset"), new XAttribute("Value", "10,10"))
        ));

        // Slider
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Slider"),
            new XElement("Setter", new XAttribute("Property", "MinimumTrackColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "MaximumTrackColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "MinimumTrackColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "MaximumTrackColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // SwipeItem
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "SwipeItem"),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource BackgroundCl}}"))
        ));

        // Switch
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "Switch"),
            new XElement("Setter", new XAttribute("Property", "OnColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{StaticResource ForegroundCl}")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "OnColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "On"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "OnColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource SecondaryCl}, Dark={StaticResource Complementary1Cl}}")),
                                new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource PrimaryCl}, Dark={StaticResource ForegroundCl}}"))
                            )
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Off"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "ThumbColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary2Cl}, Dark={StaticResource Complementary2Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // TimePicker
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "TimePicker"),
            new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "Transparent")),
            new XElement("Setter", new XAttribute("Property", "FontFamily"), new XAttribute("Value", "nfcodeRegular")),
            new XElement("Setter", new XAttribute("Property", "FontSize"), new XAttribute("Value", "14")),
            new XElement("Setter", new XAttribute("Property", "MinimumHeightRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "MinimumWidthRequest"), new XAttribute("Value", "32")),
            new XElement("Setter", new XAttribute("Property", "VisualStateManager.VisualStateGroups"),
                new XElement("VisualStateGroupList",
                    new XElement("VisualStateGroup",
                        new XAttribute("x:Name", "CommonStates"),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Normal")
                        ),
                        new XElement("VisualState",
                            new XAttribute("x:Name", "Disabled"),
                            new XElement("VisualState.Setters",
                                new XElement("Setter", new XAttribute("Property", "TextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}"))
                            )
                        )
                    )
                )
            )
        ));

        // Page
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("ApplyToDerivedTypes", "True"),
            new XAttribute("TargetType", "Page"),
            new XElement("Setter", new XAttribute("Property", "Padding"), new XAttribute("Value", "0")),
            new XElement("Setter", new XAttribute("Property", "BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource Complementary3Cl}}"))
        ));

        // Shell
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("ApplyToDerivedTypes", "True"),
            new XAttribute("TargetType", "Shell"),
            new XElement("Setter", new XAttribute("Property", "Shell.BackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.ForegroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.TitleColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource BackgroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.DisabledColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.UnselectedColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary1Cl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.NavBarHasShadow"), new XAttribute("Value", "False")),
            new XElement("Setter", new XAttribute("Property", "Shell.TabBarBackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource BackgroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.TabBarForegroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.TabBarTitleColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "Shell.TabBarUnselectedColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        // NavigationPage
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
        new XAttribute("TargetType", "NavigationPage"),
        new XElement("Setter", new XAttribute("Property", "BarBackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource Complementary3Cl}}")),
        new XElement("Setter", new XAttribute("Property", "BarTextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource ForegroundCl}}")),
        new XElement("Setter", new XAttribute("Property", "IconColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource ForegroundCl}}"))
    ));

        // TabbedPage
        mauiStylesDocument.Add(new XElement(xmlns + "Style",
            new XAttribute("TargetType", "TabbedPage"),
            new XElement("Setter", new XAttribute("Property", "BarBackgroundColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "BarTextColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource ForegroundCl}, Dark={StaticResource ForegroundCl}}")),
            new XElement("Setter", new XAttribute("Property", "UnselectedTabColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary1Cl}, Dark={StaticResource Complementary3Cl}}")),
            new XElement("Setter", new XAttribute("Property", "SelectedTabColor"), new XAttribute("Value", "{AppThemeBinding Light={StaticResource Complementary3Cl}, Dark={StaticResource Complementary1Cl}}"))
        ));

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            writer.WriteLine("<?xaml-comp compile=\"true\" ?>");
            writer.Write(mauiStylesDocument.ToString());
        }
        #endregion
    }
}
