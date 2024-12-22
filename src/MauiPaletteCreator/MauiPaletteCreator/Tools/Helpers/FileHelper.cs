namespace MauiPaletteCreator.Tools;

public class FileHelper
{
    public static async Task<string> LoadProjectFile()
    {
        var projectFile = await FilePicker.Default.PickAsync();
        if (projectFile is not null)
        {
            return projectFile.FullPath;
        }
        return string.Empty;
    }
}
