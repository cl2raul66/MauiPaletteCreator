namespace MauiPaletteCreator.Models.View;

public class ColorPaletteGroup : List<ColorPalette>
{
    public string? NameGroup { get; private set; }

    public ColorPaletteGroup(string nameGroup, IEnumerable<ColorPalette> colorPalettes) : base(colorPalettes)
    {
        NameGroup = nameGroup;
    }

    public ColorPaletteGroup(string nameGroup, ColorPalette[] colorPalettes) : base(colorPalettes)
    {
        NameGroup = nameGroup;
    }

    public ColorPaletteGroup(string nameGroup, List<ColorPalette> colorPalettes) : base(colorPalettes)
    {
        NameGroup = nameGroup;
    }
}
