using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class GoModules
{
    public static DirectoryDetection? From(DirectoryManifests directory) =>
        directory.HasGoModule ? DirectoryDetection.Found(ModuleCandidate.In(directory, BuildStack.Go, Framework.Go)) : null;
}
