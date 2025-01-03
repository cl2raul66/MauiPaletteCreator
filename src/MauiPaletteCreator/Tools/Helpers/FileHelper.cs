namespace MauiPaletteCreator.Tools;

public partial class FileHelper
{
    public static Dictionary<string, string[]> FilesToBeModified = [];

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

    public static void ApplyModifications()
    {
        foreach (var group in FilesToBeModified)
        {
            if (ModifiedFiles.TryGetValue(group.Key, out var modifiedFiles))
            {
                var filesToBeModified = group.Value;
                for (int i = 0; i < filesToBeModified.Length; i++)
                {
                    var fileToBeModified = filesToBeModified[i];
                    var modifiedFile = modifiedFiles[i];

                    if (Path.GetFileName(fileToBeModified) == Path.GetFileName(modifiedFile))
                    {
                        // Crear copia de seguridad
                        var backupFile = Path.Combine(CachePath, "Backup", fileToBeModified + ".bak");
                        File.Copy(fileToBeModified, backupFile, true);

                        // Reemplazar contenido
                        var content = File.ReadAllText(modifiedFile);
                        File.WriteAllText(fileToBeModified, content);
                    }
                }
            }
        }
    }
}
