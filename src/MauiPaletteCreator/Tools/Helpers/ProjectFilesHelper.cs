// Ignore Spelling: csproj

using System.Diagnostics;

namespace MauiPaletteCreator.Tools;

public class ProjectFilesHelper
{
    public static string? TestGalleryPath { get; private set; }

    public static Dictionary<string, string[]> FilesToBeModified = [];

    public static Dictionary<string, string> TargetPlatforms = [];

    public static async Task<bool> GeneratedTestGalleryAsync()
    {
        string projectPath = Path.Combine(FileSystem.Current.CacheDirectory, "TestGallery");

        if (!Directory.Exists(projectPath))
        {
            Directory.CreateDirectory(projectPath);
        }

        ProcessStartInfo? processInfo;

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
            using var process = Process.Start(processInfo);
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

            TestGalleryPath = Path.Combine(projectPath, "TestGallery.csproj");
            if (File.Exists(TestGalleryPath))
            {
                await ModifyFileAppShellXamlAsync();
                await ModifyFileMainPageCsAsync();
                await ModifyFileMainPageXamlAsync();
                FilesToBeModified = await ProjectAnalyzerHelper.GetFilesToBeModifiedAsync(TestGalleryPath);
                TargetPlatforms = await ProjectAnalyzerHelper.GetTargetPlatformsAsync(TestGalleryPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return false;
        }

        return true;
    }

    public static async Task<bool> RunTestGalleryAsync(string targetFramework)
    {
        if (string.IsNullOrEmpty(TestGalleryPath))
        {
            Console.WriteLine("El proyecto TestGallery no ha sido generado.");
            return false;
        }

        string? projectDirectory = Path.GetDirectoryName(TestGalleryPath);
        if (projectDirectory is null)
        {
            Console.WriteLine("La ruta del proyecto no es válida.");
            return false;
        }

        ProcessStartInfo? processInfo;

        if (OperatingSystem.IsWindows())
        {
            processInfo = new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"dotnet run --framework {targetFramework}\"")
            {
                WorkingDirectory = projectDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            processInfo = new ProcessStartInfo("/bin/bash", $"-c \"dotnet run --framework {targetFramework}\"")
            {
                WorkingDirectory = projectDirectory,
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
            using var process = Process.Start(processInfo);
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
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return false;
        }

        return true;
    }

    #region EXTRA
    static async Task ModifyFileAppShellXamlAsync()
    {
        var filePath = GetFilePath(TestGalleryPath, "AppShell.xaml");
        var text = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<Shell\r\n    x:Class=\"TestGallery.AppShell\"\r\n    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"\r\n    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"\r\n    xmlns:local=\"clr-namespace:TestGallery\"\r\n    Title=\"TestGallery\">\r\n\r\n    <ShellContent\r\n        Title=\"Home\"\r\n        ContentTemplate=\"{DataTemplate local:MainPage}\"\r\n        Route=\"MainPage\" />\r\n\r\n</Shell>\r\n";
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(text);
    }

    static async Task ModifyFileMainPageCsAsync()
    {
        var filePath = GetFilePath(TestGalleryPath, "MainPage.xaml.cs");
        var text = "namespace TestGallery;\r\n\r\npublic partial class MainPage : ContentPage\r\n{\r\n\tpublic MainPage()\r\n\t{\r\n\t\tInitializeComponent();\r\n\t}\r\n}\r\n";
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(text);
    }

    static async Task ModifyFileMainPageXamlAsync()
    {
        var filePath = GetFilePath(TestGalleryPath, "MainPage.xaml");
        var text = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<ContentPage\r\n    x:Class=\"TestGallery.MainPage\"\r\n    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"\r\n    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"\r\n    Shell.NavBarIsVisible=\"False\">\r\n\r\n    <ScrollView>\r\n        <VerticalStackLayout\r\n            Padding=\"16\"\r\n            HorizontalOptions=\"Center\"\r\n            Spacing=\"16\">\r\n            <Label\r\n                FontSize=\"Title\"\r\n                HorizontalTextAlignment=\"Center\"\r\n                Text=\"MAUI Gallery\" />\r\n            <Label\r\n                FontSize=\"Subtitle\"\r\n                HorizontalTextAlignment=\"Center\"\r\n                LineBreakMode=\"WordWrap\"\r\n                Text=\"Welcome to the .NET Multi-platform App UI gallery\"\r\n                WidthRequest=\"400\" />\r\n\r\n            <!--  Controles de Entrada  -->\r\n            <Frame>\r\n                <VerticalStackLayout Spacing=\"8\">\r\n                    <Label\r\n                        FontAttributes=\"Bold\"\r\n                        Text=\"Entry Controls\"\r\n                        TextTransform=\"Uppercase\" />\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Button:\" />\r\n                        <Button Text=\"Button\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"CheckBox:\" />\r\n                        <CheckBox />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"DatePicker:\" />\r\n                        <DatePicker />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Editor:\" />\r\n                        <Editor HeightRequest=\"100\" Placeholder=\"Editor placeholder\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Entry:\" />\r\n                        <Entry Placeholder=\"Entry placeholder\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Picker:\" />\r\n                        <Picker>\r\n                            <Picker.ItemsSource>\r\n                                <x:Array Type=\"{x:Type x:String}\">\r\n                                    <x:String>Item 1</x:String>\r\n                                    <x:String>Item 2</x:String>\r\n                                    <x:String>Item 3</x:String>\r\n                                </x:Array>\r\n                            </Picker.ItemsSource>\r\n                        </Picker>\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"RadioButton:\" />\r\n                        <RadioButton Content=\"Option 1\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"SearchBar:\" />\r\n                        <SearchBar Placeholder=\"Search...\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Slider:\" />\r\n                        <Slider\r\n                            Maximum=\"100\"\r\n                            Minimum=\"0\"\r\n                            Value=\"50\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Switch:\" />\r\n                        <Switch />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"TimePicker:\" />\r\n                        <TimePicker />\r\n                    </VerticalStackLayout>\r\n                </VerticalStackLayout>\r\n            </Frame>\r\n\r\n            <!--  Controles de Visualización  -->\r\n            <Frame>\r\n                <VerticalStackLayout Spacing=\"8\">\r\n                    <Label\r\n                        FontAttributes=\"Bold\"\r\n                        Text=\"Display Controls\"\r\n                        TextTransform=\"Uppercase\" />\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"ActivityIndicator:\" />\r\n                        <ActivityIndicator IsRunning=\"True\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"IndicatorView:\" />\r\n                        <IndicatorView Count=\"3\" Position=\"1\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Label:\" />\r\n                        <Label Text=\"Este es un Label de ejemplo\" />\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"ProgressBar:\" />\r\n                        <ProgressBar Progress=\"0.7\" />\r\n                    </VerticalStackLayout>\r\n                </VerticalStackLayout>\r\n            </Frame>\r\n\r\n            <!--  Controles de Contenedor  -->\r\n            <Frame>\r\n                <VerticalStackLayout Spacing=\"8\">\r\n                    <Label\r\n                        FontAttributes=\"Bold\"\r\n                        Text=\"Container Controls\"\r\n                        TextTransform=\"Uppercase\" />\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Border:\" />\r\n                        <Border>\r\n                            <Label Padding=\"10\" Text=\"Contenido dentro de Border\" />\r\n                        </Border>\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Frame:\" />\r\n                        <Frame Padding=\"10\">\r\n                            <Label Text=\"Contenido dentro de Frame\" />\r\n                        </Frame>\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"ListView:\" />\r\n                        <ListView HeightRequest=\"100\">\r\n                            <ListView.ItemsSource>\r\n                                <x:Array Type=\"{x:Type x:String}\">\r\n                                    <x:String>Item 1</x:String>\r\n                                    <x:String>Item 2</x:String>\r\n                                    <x:String>Item 3</x:String>\r\n                                </x:Array>\r\n                            </ListView.ItemsSource>\r\n                        </ListView>\r\n                    </VerticalStackLayout>\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"Stepper:\" />\r\n                        <Stepper />\r\n                    </VerticalStackLayout>\r\n                </VerticalStackLayout>\r\n            </Frame>\r\n\r\n            <!--  Controles de Imagen  -->\r\n            <Frame>\r\n                <VerticalStackLayout Spacing=\"8\">\r\n                    <Label\r\n                        FontAttributes=\"Bold\"\r\n                        Text=\"Image Controls\"\r\n                        TextTransform=\"Uppercase\" />\r\n\r\n                    <VerticalStackLayout Spacing=\"4\">\r\n                        <Label Text=\"ImageButton:\" />\r\n                        <ImageButton HeightRequest=\"50\" Source=\"dotnet_bot.png\" />\r\n                    </VerticalStackLayout>\r\n                </VerticalStackLayout>\r\n            </Frame>\r\n        </VerticalStackLayout>\r\n    </ScrollView>\r\n\r\n</ContentPage>\r\n";
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(text);
    }

    static string GetFilePath(string? csprojPath, string fileName)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath);
        return projectDirectory is null
            ? throw new ArgumentException("La ruta del proyecto no es válida.", nameof(csprojPath))
            : Path.Combine(projectDirectory, fileName);
    }

    #endregion
}
