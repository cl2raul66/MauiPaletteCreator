using MAUIProjectManagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPaletteCreator.Services;

public class TestProjectService
{
    readonly IProjectManager projectManagerServ;

    public TestProjectService(IProjectManager projectManager)
    {
        projectManagerServ = projectManager;
    }
}
