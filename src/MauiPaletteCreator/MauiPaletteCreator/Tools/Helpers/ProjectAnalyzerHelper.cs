// Ignore Spelling: csproj

using System.Xml.Linq;

namespace MauiPaletteCreator.Tools;

public class ProjectAnalyzerHelper
{
    public static bool IsApplicationMaui(string csprojPath)
    {
        if (!File.Exists(csprojPath) || Path.GetExtension(csprojPath) != ".csproj")
        {
            return false;
        }

        var doc = XDocument.Load(csprojPath);
        var useMaui = doc.Descendants("UseMaui").FirstOrDefault()?.Value;
        var outputType = doc.Descendants("OutputType").FirstOrDefault()?.Value;

        return useMaui == "true" && outputType == "Exe";
    }
}
