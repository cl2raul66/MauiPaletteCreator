using System.Diagnostics;

namespace MauiPaletteCreator.Tools;

public partial class FileHelper
{
    public static async Task<bool> GeneratedTestGalleryAsync()
    {
        string projectPath = Path.Combine(FileSystem.Current.CacheDirectory, "TestGallery");

        if (!Directory.Exists(projectPath))
        {
            Directory.CreateDirectory(projectPath);
        }

        ProcessStartInfo? processInfo = null;

        if (OperatingSystem.IsWindows())
        {
            processInfo = new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"dotnet new maui -o {projectPath}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            processInfo = new ProcessStartInfo("/bin/bash", $"-c \"dotnet new maui -o {projectPath}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else
        {
            Console.WriteLine("El sistema operativo no es compatible.");
            return false;
        }

        try
        {
            using (var process = Process.Start(processInfo))
            {
                if (process is null)
                {
                    Console.WriteLine("No se pudo iniciar el proceso.");
                    return false;
                }

                await process.WaitForExitAsync();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                Console.WriteLine(output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return false;
        }

        return true;
    }

}
