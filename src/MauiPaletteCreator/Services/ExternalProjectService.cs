namespace MauiPaletteCreator.Services;

public interface IExternalProjectService
{
    Dictionary<string, string[]>? FilesToBeModified { get; set; }
    bool IsLoaded { get; }

    Task LoadProjectAsync(string projectPath);
}

public class ExternalProjectService : IExternalProjectService
{
    readonly IProjectManager projectManagerServ;

    public Dictionary<string, string[]>? FilesToBeModified { get; set; }

    public ExternalProjectService(IProjectManager projectManager)
    {
        projectManagerServ = projectManager;
    }

    public bool IsLoaded { get; private set; }

    public async Task LoadProjectAsync(string projectPath)
    {
        await projectManagerServ.SetProjectDirectory(projectPath);
        await projectManagerServ.RestoreAsync();
        await projectManagerServ.BuildAsync();
    }
}
