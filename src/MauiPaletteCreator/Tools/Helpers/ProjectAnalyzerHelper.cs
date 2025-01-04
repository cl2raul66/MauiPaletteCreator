// Ignore Spelling: csproj

using System.Xml.Linq;

namespace MauiPaletteCreator.Tools;

public class ProjectAnalyzerHelper
{
    public static async Task<bool> IsApplicationMauiAsync(string csprojPath)
    {
        if (!File.Exists(csprojPath) || Path.GetExtension(csprojPath) != ".csproj")
        {
            return false;
        }

        var doc = await XDocument.LoadAsync(File.OpenRead(csprojPath), LoadOptions.None, CancellationToken.None);
        var useMaui = doc.Descendants("UseMaui").FirstOrDefault()?.Value;
        var outputType = doc.Descendants("OutputType").FirstOrDefault()?.Value;

        var result = useMaui == "true" && outputType == "Exe";

        return result;
    }

    public static async Task<Dictionary<string, string[]>> GetFilesToBeModifiedAsync(string? csprojPath)
    {
        if (string.IsNullOrEmpty(csprojPath))
        {
            return [];
        }

        Dictionary<string, string[]> result = [];
        var projectDirectory = Path.GetDirectoryName(csprojPath);
        if (projectDirectory is not null)
        {
            var mauiFiles = new[]
            {
                Path.Combine(projectDirectory, "Resources", "Styles", "Colors.xaml"),
                Path.Combine(projectDirectory, "Resources", "Styles", "Styles.xaml")
            };

            var androidFiles = new[]
            {
                Path.Combine(projectDirectory, "Platforms", "Android", "Resources", "values", "colors.xml")
            };

            result["MAUI"] = mauiFiles.Where(File.Exists).ToArray();
            result["Android"] = androidFiles.Where(File.Exists).ToArray();
        }
        return await Task.FromResult(result);
    }

    public static async Task<Dictionary<string, string>> GetTargetPlatformsAsync(string? csprojPath)
    {
        if (string.IsNullOrEmpty(csprojPath))
        {
            return [];
        }

        Dictionary<string, string> result = [];
        var doc = await XDocument.LoadAsync(File.OpenRead(csprojPath), LoadOptions.None, CancellationToken.None);
        var targetFrameworks = doc.Descendants("TargetFrameworks").FirstOrDefault()?.Value;

        if (targetFrameworks is not null)
        {
            var frameworks = targetFrameworks.Split(';');
            foreach (var framework in frameworks)
            {
                if (framework.Contains("net"))
                {
                    if (framework.Contains("android"))
                    {
                        result["Android"] = framework;
                    }
                    else if (framework.Contains("ios"))
                    {
                        result["iOS"] = framework;
                    }
                    else if (framework.Contains("maccatalyst"))
                    {
                        result["MacCatalyst"] = framework;
                    }
                    else if (framework.Contains("windows"))
                    {
                        result["Windows"] = framework;
                    }
                    else if (framework.Contains("tizen"))
                    {
                        result["Tizen"] = framework;
                    }
                }
            }
        }

        // Check for additional TargetFrameworks with conditions
        var conditionalFrameworks = doc.Descendants("TargetFrameworks")
            .Where(e => e.Attribute("Condition") != null)
            .Select(e => e.Value.Split(';'))
            .SelectMany(f => f)
            .Distinct();

        foreach (var framework in conditionalFrameworks)
        {
            if (framework.Contains("net"))
            {
                if (framework.Contains("android"))
                {
                    result["Android"] = framework;
                }
                else if (framework.Contains("ios"))
                {
                    result["iOS"] = framework;
                }
                else if (framework.Contains("maccatalyst"))
                {
                    result["MacCatalyst"] = framework;
                }
                else if (framework.Contains("windows"))
                {
                    result["Windows"] = framework;
                }
                else if (framework.Contains("tizen"))
                {
                    result["Tizen"] = framework;
                }
            }
        }

        return await Task.FromResult(result);
    }
}
