using System.Text.RegularExpressions;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Manifests;

public static partial class ManifestFiles
{
    private static readonly HashSet<string> names =
    [
        "package.json",
        "go.mod",
        "go.work",
        "pom.xml",
        "build.gradle",
        "build.gradle.kts",
        "requirements.txt",
        "pyproject.toml",
        "Pipfile",
        "Cargo.toml",
        "angular.json",
        "pnpm-workspace.yaml",
        "Dockerfile",
    ];

    private static readonly string[] suffixes = [".csproj", ".fsproj", ".sln", ".slnx"];

    public static bool IsManifest(this string path) =>
        !path.IsIgnored() && (names.Contains(path.FileNameOf()) || suffixes.Any(path.EndsWith));

    public static bool IsCompose(this string path) => !path.IsIgnored() && ComposePattern().IsMatch(path.FileNameOf());

    public static bool IsReadable(this string path) => path.IsManifest() || path.IsCompose();

    [GeneratedRegex(@"^(docker-)?compose(\..+)?\.ya?ml$")]
    private static partial Regex ComposePattern();
}
