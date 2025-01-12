// Ignore Spelling: csproj

namespace MauiPaletteCreator.Tools;

public class ProjectAnalyzerHelper
{
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
}
