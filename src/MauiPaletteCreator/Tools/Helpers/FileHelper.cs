// Ignore Spelling: csproj

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

    public static void SetFilesToBeModified(string projectPath, Dictionary<string, string[]> filesToBeModified)
    {
        var projectDirectory = Path.GetDirectoryName(projectPath);
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
        foreach (var group in filesToBeModified) // Iteramos sobre los archivos a modificar
        {
            if (ModifiedFiles.TryGetValue(group.Key, out var modifiedFiles)) // Verificamos si existe la clave en ModifiedFiles
            {
                var filesToModify = group.Value;

                // Verificamos que ambos arrays tengan la misma longitud
                if (filesToModify.Length != modifiedFiles.Length)
                {
                    return;
                }

                for (int i = 0; i < filesToModify.Length; i++)
                {
                    var fileToBeModified = filesToModify[i];
                    var modifiedFile = modifiedFiles[i];

                    if (Path.GetFileName(fileToBeModified) == Path.GetFileName(modifiedFile))
                    {
                        // Opcional: crear copia de seguridad
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
