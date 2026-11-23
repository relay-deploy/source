using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class PythonModules
{
    public static DirectoryDetection? From(DirectoryManifests directory) =>
        directory.Python is not { } python
            ? null
            : DirectoryDetection.Found(ModuleCandidate.In(directory, BuildStack.Python, FrameworkOf(python)));

    private static Framework? FrameworkOf(PythonManifest python) => python switch
    {
        _ when python.Declares("django") => Framework.Django,
        _ when python.Declares("fastapi") => Framework.FastApi,
        _ when python.Declares("flask") => Framework.Flask,
        _ => null,
    };
}
