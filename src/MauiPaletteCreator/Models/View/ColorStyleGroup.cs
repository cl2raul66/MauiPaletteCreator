namespace MauiPaletteCreator.Models;

public class ColorStyleGroup : List<ColorStyle>
{
    public string? Key { get; private set; }

    public ColorStyleGroup(string nameGroup, IEnumerable<ColorStyle> colorStyles) : base(colorStyles)
    {
        Key = nameGroup;
    }

    public ColorStyleGroup(string nameGroup, ColorStyle[] colorStyles) : base(colorStyles)
    {
        Key = nameGroup;
    }

    public ColorStyleGroup(string nameGroup, List<ColorStyle> colorStyles) : base(colorStyles)
    {
        Key = nameGroup;
    }
}
