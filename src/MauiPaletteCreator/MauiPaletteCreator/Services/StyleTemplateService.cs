using MauiPaletteCreator.Models;
using MauiPaletteCreator.Tools;

namespace MauiPaletteCreator.Services;

public interface IStyleTemplateService
{
    IEnumerable<ColorStyle> GetDefaultDarkColorStyle();
    IEnumerable<ColorStyle> GetDefaultLightColorStyle();
    IEnumerable<ColorStyle> GetDefaultNeutralDarkColorStyle();
    IEnumerable<ColorStyle> GetDefaultNeutralLightColorStyle();
    IEnumerable<ColorStyle> GetDefaultPrincipalDarkColorStyle();
    IEnumerable<ColorStyle> GetDefaultPrincipalLightColorStyle();
    IEnumerable<ColorStyle> GetDefaultSemanticDarkColorStyle();
    IEnumerable<ColorStyle> GetDefaultSemanticLightColorStyle();
    IEnumerable<ColorStyle> InsertPrincipalLight(ColorStyle colorStyle);
}

public class StyleTemplateService : IStyleTemplateService
{
    List<ColorStyle> PrincipalLightColorStyle = [
        new () { Name = "PrimaryCl",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light, Locked = false},
        new () { Name = "SecondaryCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Light, Locked = false},
        new() { Name = "AccentCl", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Light, Locked = false }
    ];
    List<ColorStyle> SemanticLightColorStyle = [
        new () { Name = "ErrorCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Light, Locked = false},
        new () { Name = "SuccessCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Light, Locked = false },
        new() { Name = "WarningCl", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Light, Locked = false }
    ];
    List<ColorStyle> NeutralLightColorStyle = [
        new() { Name = "ForegroundCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Light, Locked = false },
        new() { Name = "BackgroundCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Light, Locked = false },
        new() { Name = "Gray250Cl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Light, Locked = false },
        new() { Name = "Gray500Cl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Light, Locked = false },
        new() { Name = "Gray750Cl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Light, Locked = false }
    ];

    List<ColorStyle> PrincipalDarkColorStyle = [
        new () { Name = "PrimaryDarkCl",Value = Color.Parse("#FF512BD4"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark, Locked = false},
        new () { Name = "SecondaryDarkCl", Value = Color.Parse("#FF2B0B98"), Tag="PRINCIPAL", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "AccentDarkCl", Value = Color.Parse("#FF2D6FCC"), Tag = "PRINCIPAL", Scheme = ColorScheme.Dark, Locked = false }
    ];
    List<ColorStyle> SemanticDarkColorStyle = [
        new () { Name = "ErrorDarkCl", Value = Color.Parse("#FFFF0000"), Tag="SEMANTIC", Scheme = ColorScheme.Dark, Locked = false},
        new () { Name = "SuccessDarkCl", Value = Color.Parse("#FF00FF00"), Tag="SEMANTIC", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "WarningDarkCl", Value = Color.Parse("#FFFFFF00"), Tag = "SEMANTIC", Scheme = ColorScheme.Dark, Locked = false }
    ];
    List<ColorStyle> NeutralDarkColorStyle = [
        new() { Name = "ForegroundDarkCl", Value = Color.Parse("#FFF7F5FF"), Tag="NEUTRAL", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "BackgroundDarkCl", Value = Color.Parse("#FF23135E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "Gray250DarkCl", Value = Color.Parse("#FFE1E1E1"), Tag="NEUTRAL", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "Gray500DarkCl", Value = Color.Parse("#FFACACAC"), Tag="NEUTRAL", Scheme = ColorScheme.Dark, Locked = false },
        new() { Name = "Gray750DarkCl", Value = Color.Parse("#FF6E6E6E"), Tag="NEUTRAL", Scheme = ColorScheme.Dark, Locked = false }
    ];

    public IEnumerable<ColorStyle> GetDefaultPrincipalLightColorStyle() => PrincipalLightColorStyle;
    public IEnumerable<ColorStyle> InsertPrincipalLight(ColorStyle colorStyle)
    {
        var ele = PrincipalLightColorStyle.FirstOrDefault(x => x.Name == colorStyle.Name);
        if (ele is null)
        {
            return [];
        }
        var idx = PrincipalLightColorStyle.IndexOf(ele);
        PrincipalLightColorStyle[idx] = colorStyle;
        return PrincipalLightColorStyle;
    }
    public IEnumerable<ColorStyle> GetDefaultSemanticLightColorStyle() => SemanticLightColorStyle;
    public IEnumerable<ColorStyle> GetDefaultNeutralLightColorStyle() => NeutralLightColorStyle;

    public IEnumerable<ColorStyle> GetDefaultPrincipalDarkColorStyle() => PrincipalDarkColorStyle;
    public IEnumerable<ColorStyle> GetDefaultSemanticDarkColorStyle() => SemanticDarkColorStyle;
    public IEnumerable<ColorStyle> GetDefaultNeutralDarkColorStyle() => NeutralDarkColorStyle;

    public IEnumerable<ColorStyle> GetDefaultLightColorStyle()
    {
        return PrincipalLightColorStyle.Concat(SemanticLightColorStyle).Concat(NeutralLightColorStyle);
    }
    public IEnumerable<ColorStyle> GetDefaultDarkColorStyle()
    {
        return PrincipalDarkColorStyle.Concat(SemanticDarkColorStyle).Concat(NeutralDarkColorStyle);
    }
}
