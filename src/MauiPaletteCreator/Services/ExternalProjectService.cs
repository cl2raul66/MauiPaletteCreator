
using MAUIProjectManagerLib;

namespace MauiPaletteCreator.Services;

public interface IExternalProjectService
{
    Dictionary<string, string[]> FilesToBeModified { get; set; }
    bool IsLoaded { get; }
    string ProjectPath { get; }
    Dictionary<string, string> TargetPlatforms { get; }

    Task LoadProjectAsync(string projectPath);
}

public class ExternalProjectService : IExternalProjectService
{
    readonly IProjectManager projectManagerServ;

    public string ProjectPath { get; private set; }

    public Dictionary<string, string> TargetPlatforms { get; private set; }

    public Dictionary<string, string[]> FilesToBeModified { get; set; }

    public ExternalProjectService()
    {
        projectManagerServ = new ProjectManager();
        ProjectPath = string.Empty;
        TargetPlatforms = [];
        FilesToBeModified = [];
    }

    public bool IsLoaded { get; private set; }

    public async Task LoadProjectAsync(string projectPath)
    {
        await projectManagerServ.SetProjectDirectory(projectPath);
        TargetPlatforms = await projectManagerServ.GetTargetPlatformsAsync();
        //await projectManagerServ.BuildAsync();
        ProjectPath = projectManagerServ.ProjectPath ?? string.Empty;
        IsLoaded = !string.IsNullOrEmpty(ProjectPath) && TargetPlatforms.Count > 0;
    }
}
