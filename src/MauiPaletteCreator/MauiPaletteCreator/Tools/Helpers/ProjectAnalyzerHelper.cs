// Ignore Spelling: csproj

using System.Xml.Linq;

namespace MauiPaletteCreator.Tools;

public class ProjectAnalyzerHelper
{
    public static Dictionary<string, string> TargetPlatforms = [];

    public static bool IsApplicationMaui(string csprojPath)
    {
        if (!File.Exists(csprojPath) || Path.GetExtension(csprojPath) != ".csproj")
        {
            return false;
        }

        var doc = XDocument.Load(csprojPath);
        var useMaui = doc.Descendants("UseMaui").FirstOrDefault()?.Value;
        var outputType = doc.Descendants("OutputType").FirstOrDefault()?.Value;

        var result = useMaui == "true" && outputType == "Exe";

        if (result)
        {
            SetFilesToBeModified(csprojPath);
            SetTargetPlatforms(csprojPath);
        }

        return result;
    }

    #region EXTRA
    static void SetFilesToBeModified(string csprojPath)
    {
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

            var iosFiles = new[]
            {
                    Path.Combine(projectDirectory, "Platforms", "iOS", "AppDelegate.cs"),
                    Path.Combine(projectDirectory, "Platforms", "iOS", "Info.plist")
                };

            FileHelper.FilesToBeModified["MAUI"] = [.. mauiFiles.Where(File.Exists)];
            FileHelper.FilesToBeModified["Android"] = [.. androidFiles.Where(File.Exists)];
            FileHelper.FilesToBeModified["iOS"] = [.. iosFiles.Where(File.Exists)];
        }
    }

    static void SetTargetPlatforms(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
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
                        TargetPlatforms["Android"] = framework;
                    }
                    else if (framework.Contains("ios"))
                    {
                        TargetPlatforms["iOS"] = framework;
                    }
                    else if (framework.Contains("maccatalyst"))
                    {
                        TargetPlatforms["MacCatalyst"] = framework;
                    }
                    else if (framework.Contains("windows"))
                    {
                        TargetPlatforms["Windows"] = framework;
                    }
                    else if (framework.Contains("tizen"))
                    {
                        TargetPlatforms["Tizen"] = framework;
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
                    TargetPlatforms["Android"] = framework;
                }
                else if (framework.Contains("ios"))
                {
                    TargetPlatforms["iOS"] = framework;
                }
                else if (framework.Contains("maccatalyst"))
                {
                    TargetPlatforms["MacCatalyst"] = framework;
                }
                else if (framework.Contains("windows"))
                {
                    TargetPlatforms["Windows"] = framework;
                }
                else if (framework.Contains("tizen"))
                {
                    TargetPlatforms["Tizen"] = framework;
                }
            }
        }
    }
    #endregion
}
