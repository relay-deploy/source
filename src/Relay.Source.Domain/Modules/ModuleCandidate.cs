using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public sealed record ModuleCandidate(
    string Path,
    BuildStack Stack,
    Framework? Framework,
    BuildMode Mode,
    int? PortGuess,
    double Confidence,
    bool Exposed,
    bool WorkspaceBuild,
    BuildTool? Tool,
    string? WebServerRoot,
    BuildSelector? Selector)
{
    public static ModuleCandidate In(DirectoryManifests directory, BuildStack stack, Framework? framework) =>
        For(directory.Directory, stack, framework, directory.HasDockerfile ? BuildMode.Dockerfile : BuildMode.Buildpack);

    public static ModuleCandidate For(string path, BuildStack stack, Framework? framework, BuildMode mode) =>
        new(
            path,
            stack,
            framework,
            mode,
            framework?.DefaultPort() ?? stack.FallbackPort(),
            1.0,
            ModuleExposure.IsExposedByDefault(path),
            false,
            null,
            null,
            null);
}
