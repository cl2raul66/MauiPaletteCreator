using MAUIProjectManagerLib;

namespace MauiPaletteCreator.Services;

public interface ITestProjectService
{
    Dictionary<string, string[]> FilesToBeModified { get; set; }
    bool IsCreated { get; }
    string ProjectPath { get; }
    Dictionary<string, string> TargetPlatforms { get; }

    Task CreateProjectAsync(string projectPath);
    Task RunProjectAsync(string platform);
}

public class TestProjectService : ITestProjectService
{
    readonly IProjectManager projectManagerServ;

    public string ProjectPath { get; private set; }

    public Dictionary<string, string> TargetPlatforms { get; private set; }

    public Dictionary<string, string[]> FilesToBeModified { get; set; }

    public TestProjectService()
    {
        projectManagerServ = new ProjectManager();
        ProjectPath = string.Empty;
        TargetPlatforms = [];
        FilesToBeModified = [];
    }

    public bool IsCreated { get; private set; }

    public async Task CreateProjectAsync(string projectPath)
    {
        await projectManagerServ.SetProjectDirectory(projectPath);
        await projectManagerServ.CreateAsync();
        //await projectManagerServ.BuildAsync();

        if (!string.IsNullOrEmpty(projectManagerServ.ProjectPath))
        {
            IsCreated = true;
            ProjectPath = projectManagerServ.ProjectPath;
            TargetPlatforms = await projectManagerServ.GetTargetPlatformsAsync();
            await ModifyFileAppShellXamlAsync();
            await ModifyFileMainPageCsAsync();
            await ModifyFileMainPageXamlAsync();
            await projectManagerServ.RestoreAsync();
            await projectManagerServ.BuildAsync();
        }
    }

    public async Task RunProjectAsync(string platform)
    {
        string targetFramework = TargetPlatforms[platform];
        await projectManagerServ.RunAsync(targetFramework);
    }

    public async Task DeletedProjectAsync()
    {

    }

    #region EXTRA
    async Task ModifyFileAppShellXamlAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("AppShell.xaml.txt");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        var filePath = GetFilePath("AppShell.xaml");
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(contents);
    }

    async Task ModifyFileMainPageCsAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("MainPage.xaml.cs.txt");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        var filePath = GetFilePath("MainPage.xaml.cs");
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(contents);
    }

    async Task ModifyFileMainPageXamlAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("MainPage.xaml.txt");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();

        var filePath = GetFilePath("MainPage.xaml");
        using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync(contents);
    }

    //async Task ModifyFileMauiStylesAsync()
    //{
    //    using var stream = await FileSystem.OpenAppPackageFileAsync("MainPage.xaml");
    //    using var reader = new StreamReader(stream);
    //    var contents = await reader.ReadToEndAsync();

    //    var filePath = GetFilePath("MainPage.xaml");       
    //    using var writer = new StreamWriter(filePath);
    //    await writer.WriteLineAsync(contents);
    //}

    string GetFilePath(string fileName)
    {
        var projectDirectory = Path.GetDirectoryName(projectManagerServ.ProjectPath);

        return projectDirectory is null
            ? string.Empty
            : Path.Combine(projectDirectory, fileName);
    }
    #endregion
}
