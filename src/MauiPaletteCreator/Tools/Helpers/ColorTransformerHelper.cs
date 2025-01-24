namespace MauiPaletteCreator.Tools;

public static class ColorTransformer
{
    public static (double H, double S, double L) RgbToHsl(Color color)
    {
        return (color.GetHue(), color.GetSaturation(), color.GetLuminosity());
    }

    public static Color HslToRgb(double h, double s, double l)
    {
        return Color.FromHsla(h / 360.0, s, l);
    }

    public static Color AdjustLuminosity(Color color, float adjustment)
    {
        float newL = Math.Min(1, Math.Max(0, color.GetLuminosity() + adjustment));
        return color.WithLuminosity(newL);
    }

    public static Color[] GeneratePalette(List<Color> palette, float luminosityAdjustment) => GeneratePalette(palette, luminosityAdjustment);

    public static Color[] GeneratePalette(Color[] palette, float luminosityAdjustment)
    {
        List<Color> adjustedPalette = [];

        foreach (var color in palette)
        {
            Color adjustedColor = AdjustLuminosity(color, luminosityAdjustment);
            adjustedPalette.Add(adjustedColor);
        }

        return [.. adjustedPalette];
    }
}
