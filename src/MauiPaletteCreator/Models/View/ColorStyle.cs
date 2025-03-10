﻿using MauiPaletteCreator.Tools;

namespace MauiPaletteCreator.Models.View;

public class ColorStyle
{
    public string? Name { get; set; }
    public Color? Value { get; set; }
    public string? Tag { get; set; }
    public ColorScheme Scheme { get; set; }
}
