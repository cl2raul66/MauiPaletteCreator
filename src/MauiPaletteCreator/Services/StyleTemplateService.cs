using MauiPaletteCreator.Models;
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
        new() { Name = "cl:Primary",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Secondary", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Accent", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light }
    ];
    List<ColorStyle> SemanticLightColorStyle = [
        new() { Name = "cl:Error", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
        new() { Name = "cl:Success", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light },
        new() { Name = "cl:Warning", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Light }
    ];
    List<ColorStyle> NeutralLightColorStyle = [
        new() { Name = "cl:Foreground", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Background", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Complementary1", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Complementary2", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Light },
        new() { Name = "cl:Complementary3", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Light }
    ];

    List<ColorStyle> PrincipalDarkColorStyle = [
        new() { Name = "cl:PrimaryDark",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:SecondaryDark", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:AccentDark", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark }
    ];
    List<ColorStyle> SemanticDarkColorStyle = [
        new() { Name = "cl:ErrorDark", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Dark },
        new() { Name = "cl:SuccessDark", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Dark },
        new() { Name = "cl:WarningDark", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Dark }
    ];
    List<ColorStyle> NeutralDarkColorStyle = [
        new() { Name = "cl:ForegroundDark", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:BackgroundDark", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:Complementary1Dark", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:Complementary2Dark", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Dark },
        new() { Name = "cl:Complementary3Dark", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark }
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
        var colorPrimary = lightColorStyles.First(x => x.Name == "cl:Primary")?.Value?.ToArgbHex();
        var colorPrimaryDark = darkColorStyles.First(x => x.Name == "cl:PrimaryDark")?.Value?.ToArgbHex();
        var colorAccent = lightColorStyles.First(x => x.Name == "cl:Accent")?.Value?.ToArgbHex();

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
        using var stream = await FileSystem.OpenAppPackageFileAsync("Styles.xaml.txt");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(contents);
    }
    #endregion
}
