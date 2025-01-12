namespace MauiPaletteCreator.Services;

public interface IExternalProjectService
{
    bool IsLoaded { get; }

    Task LoadProjectAsync(string projectPath);
}

public class ExternalProjectService : IExternalProjectService
{
    readonly IProjectManager projectManagerServ;

    Dictionary<string, string[]> FilesToBeModified = [];

    Dictionary<string, string[]> ModifiedFiles = [];

    Dictionary<string, string> TargetPlatforms = [];

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
        TargetPlatforms = await projectManagerServ.GetTargetPlatformsAsync();
        IsLoaded = TargetPlatforms.Count > 0;
        //if (TargetPlatforms.Count > 0)
        //{
        //    FilesToBeModified = await ProjectAnalyzerHelper.GetFilesToBeModifiedAsync(projectPath);
        //}
    }
}
