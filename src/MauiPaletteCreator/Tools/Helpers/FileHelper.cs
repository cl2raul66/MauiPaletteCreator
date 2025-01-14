namespace MauiPaletteCreator.Tools;

public partial class FileHelper
{
    public static Dictionary<string, string[]> ModifiedFiles = [];

    public static string CachePath => FileSystem.Current.CacheDirectory;

    public static async Task<string> LoadProjectFile()
    {
        var projectFile = await FilePicker.Default.PickAsync();
        if (projectFile is not null)
        {
            return projectFile.FullPath;
        }
        return string.Empty;
    }

    public static void SetFilesToBeModified(string csprojPath, Dictionary<string, string[]> filesToBeModified)
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

            //var iosFiles = new[]
            //{
            //    Path.Combine(projectDirectory, "Platforms", "iOS", "AppDelegate.cs"),
            //    Path.Combine(projectDirectory, "Platforms", "iOS", "Info.plist")
            //};

            filesToBeModified["MAUI"] = [.. mauiFiles.Where(File.Exists)];
            filesToBeModified["Android"] = [.. androidFiles.Where(File.Exists)];
            //filesToBeModified["iOS"] = [.. iosFiles.Where(File.Exists)];
        }
    }

    public static async Task ApplyModificationsAsync(Dictionary<string, string[]> filesToBeModified)
    {
        foreach (var group in ModifiedFiles)
        {
            if (filesToBeModified.TryGetValue(group.Key, out var modifiedFiles))
            {
                var filesToBeModifiedValues = group.Value;
                for (int i = 0; i < filesToBeModifiedValues.Length; i++)
                {
                    var fileToBeModified = filesToBeModifiedValues[i];
                    var modifiedFile = modifiedFiles[i];

                    if (Path.GetFileName(fileToBeModified) == Path.GetFileName(modifiedFile))
                    {
                        // Crear copia de seguridad
                        //var backupFile = Path.Combine(CachePath, "Backup", fileToBeModified + ".bak");
                        //File.Copy(fileToBeModified, backupFile, true);

                        // Reemplazar contenido
                        var content = await File.ReadAllTextAsync(modifiedFile);
                        await File.WriteAllTextAsync(fileToBeModified, content);
                    }
                }
            }
        }
    }
}
