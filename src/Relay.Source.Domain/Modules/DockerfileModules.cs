using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class DockerfileModules
{
    public static DirectoryDetection? From(DirectoryManifests directory) =>
        directory.HasDockerfile
            ? DirectoryDetection.Found(
                ModuleCandidate.For(directory.Directory, BuildStack.Custom, null, BuildMode.Dockerfile) with { Confidence = 0.6 })
            : null;
}
