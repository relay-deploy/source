using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class DotnetModules
{
    public static DirectoryDetection? From(DirectoryManifests directory)
    {
        if (directory.Projects.Count == 0)
        {
            return null;
        }

        return directory.Projects.Any(project => project.IsDeployable)
            ? DirectoryDetection.Found(ModuleCandidate.In(directory, BuildStack.Dotnet, Framework.Dotnet))
            : DirectoryDetection.Skipped(DetectionNotes.LibrarySkipped(directory.Directory.Length == 0 ? "." : directory.Directory, ".NET"));
    }
}
