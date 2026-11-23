namespace Relay.Source.Domain.Manifests;

public sealed record DirectoryManifests(
    string Directory,
    IReadOnlyList<string> FileNames,
    PackageManifest? Package,
    JvmManifest? Jvm,
    CargoManifest? Cargo,
    PythonManifest? Python,
    AngularManifest? Angular,
    PnpmWorkspaceManifest? Pnpm,
    GoWorkManifest? GoWork,
    IReadOnlyList<DotnetProjectManifest> Projects,
    IReadOnlyList<SolutionManifest> Solutions)
{
    public bool HasFile(string name) => FileNames.Contains(name);

    public bool HasGoModule => HasFile("go.mod");

    public bool HasDockerfile => HasFile("Dockerfile");

    public bool HasFilePrefixed(string prefix) => FileNames.Any(name => name.StartsWith(prefix, StringComparison.Ordinal));
}
