namespace MauiPaletteCreator.Tools;

public partial class FileHelper
{
    public static Dictionary<string, string[]> FilesToBeModified = [];

    public static Dictionary<string, string[]> ModifiedFiles = [];

    public static Dictionary<string, string> TargetPlatforms = [];

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

    public static async Task ApplyModificationsAsync(Dictionary<string, string[]>? filesToBeModified = null)
    {
        foreach (var group in filesToBeModified ?? FilesToBeModified)
        {
            if (ModifiedFiles.TryGetValue(group.Key, out var modifiedFiles))
            {
                var filesToBeModifiedValues = group.Value;
                for (int i = 0; i < filesToBeModifiedValues.Length; i++)
                {
                    var fileToBeModified = filesToBeModifiedValues[i];
                    var modifiedFile = modifiedFiles[i];

                    if (Path.GetFileName(fileToBeModified) == Path.GetFileName(modifiedFile))
                    {
                        // Crear copia de seguridad
                        var backupFile = Path.Combine(CachePath, "Backup", fileToBeModified + ".bak");
                        File.Copy(fileToBeModified, backupFile, true);

                        // Reemplazar contenido
                        var content = await File.ReadAllTextAsync(modifiedFile);
                        await File.WriteAllTextAsync(fileToBeModified, content);
                    }
                }
            }
        }
    }
}
