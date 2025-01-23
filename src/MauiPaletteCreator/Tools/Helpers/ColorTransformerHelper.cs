using Microsoft.Maui.Graphics;

namespace MauiPaletteCreator.Tools;

public static class ColorTransformer
{
    // Método para convertir un color RGB a HSL
    public static (double H, double S, double L) RgbToHsl(Color color)
    {
        return (color.GetHue(), color.GetSaturation(), color.GetLuminosity());
    }

    // Método para convertir un color HSL a RGB
    public static Color HslToRgb(double h, double s, double l)
    {
        return Color.FromHsla(h / 360.0, s, l);
    }

    // Método para ajustar la luminosidad de un color HSL
    public static Color AdjustLuminosity(Color color, float adjustment)
    {
        float newL = Math.Min(1, Math.Max(0, color.GetLuminosity() + adjustment));
        return color.WithLuminosity(newL);
    }

    // Método para generar una paleta de colores oscuros a partir de una paleta clara
    public static List<Color> GenerateDarkPalette(List<Color> lightPalette, double luminosityAdjustment = -0.4)
    {
        return GeneratePalette(lightPalette, (float)luminosityAdjustment);
    }

    // Método para generar una paleta de colores claros a partir de una paleta oscura
    public static List<Color> GenerateLightPalette(List<Color> darkPalette, double luminosityAdjustment = 0.4)
    {
        return GeneratePalette(darkPalette, (float)luminosityAdjustment);
    }

    // Método genérico para generar una paleta de colores ajustando la luminosidad
    private static List<Color> GeneratePalette(List<Color> palette, float luminosityAdjustment)
    {
        List<Color> adjustedPalette = new List<Color>();

        foreach (var color in palette)
        {
            Color adjustedColor = AdjustLuminosity(color, luminosityAdjustment);
            adjustedPalette.Add(adjustedColor);
        }

        return adjustedPalette;
    }
}
