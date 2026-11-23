using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Modules;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Manifests;

public sealed class ManifestSet
{
    private static readonly string[] pythonFiles = ["requirements.txt", "pyproject.toml", "Pipfile"];

    private readonly Dictionary<string, DirectoryManifests> directories = [];

    private readonly List<DetectionNote> notes = [];

    private ManifestSet(RepositorySnapshot snapshot)
    {
        foreach (var directory in snapshot.Directories())
        {
            var names = snapshot.NamesIn(directory);
            if (names.Any(name => name.IsManifest()))
            {
                directories[directory] = Read(snapshot, directory, names);
            }
        }
    }

    public IReadOnlyCollection<DirectoryManifests> Directories => directories.Values;

    public IReadOnlyList<DetectionNote> Notes => notes;

    public static ManifestSet From(RepositorySnapshot snapshot) => new(snapshot);

    public DirectoryManifests? At(string directory) => directories.GetValueOrDefault(directory);

    private DirectoryManifests Read(RepositorySnapshot snapshot, string directory, IReadOnlyList<string> names)
    {
        var python = pythonFiles.FirstOrDefault(file => names.Contains(file));
        var jvm = Parse(snapshot, directory, "pom.xml", JvmManifest.Parse)
            ?? Parse(snapshot, directory, "build.gradle", JvmManifest.Parse)
            ?? Parse(snapshot, directory, "build.gradle.kts", JvmManifest.Parse);
        var usesGradle = names.Contains("build.gradle") || names.Contains("build.gradle.kts");
        return new DirectoryManifests(
            directory,
            names,
            Parse(snapshot, directory, "package.json", PackageManifest.Parse),
            jvm is not null && usesGradle ? jvm with { Tool = BuildTool.Gradle } : jvm,
            Content(snapshot, directory, "Cargo.toml") is { } cargo ? CargoManifest.Parse(cargo) : null,
            python is null ? null : PythonManifest.Parse(python, Content(snapshot, directory, python) ?? string.Empty),
            Parse(snapshot, directory, "angular.json", AngularManifest.Parse),
            Content(snapshot, directory, "pnpm-workspace.yaml") is { } pnpm ? PnpmWorkspaceManifest.Parse(pnpm) : null,
            Content(snapshot, directory, "go.work") is { } goWork ? GoWorkManifest.Parse(goWork) : null,
            [.. ParseAll(snapshot, directory, names, ".csproj", ".fsproj", DotnetProjectManifest.Parse)],
            [.. ParseAll(snapshot, directory, names, ".sln", ".slnx", SolutionManifest.Parse)]);
    }

    private static string? Content(RepositorySnapshot snapshot, string directory, string name) =>
        snapshot.FileAt(Combine(directory, name))?.Content;

    private static string Combine(string directory, string name) => directory.Length == 0 ? name : $"{directory}/{name}";

    private T? Parse<T>(RepositorySnapshot snapshot, string directory, string name, Func<string, string, Result<T>> parse)
        where T : class
    {
        var path = Combine(directory, name);
        var file = snapshot.FileAt(path);
        if (file is null)
        {
            return null;
        }

        var parsed = parse(path, file.Content);
        if (parsed.IsSuccess)
        {
            return parsed.Value;
        }

        notes.Add(DetectionNote.From(parsed.Error, path));
        return null;
    }

    private IEnumerable<T> ParseAll<T>(
        RepositorySnapshot snapshot,
        string directory,
        IReadOnlyList<string> names,
        string firstSuffix,
        string secondSuffix,
        Func<string, string, Result<T>> parse)
        where T : class =>
        names
            .Where(name => name.EndsWith(firstSuffix, StringComparison.Ordinal) || name.EndsWith(secondSuffix, StringComparison.Ordinal))
            .Select(name => Parse(snapshot, directory, name, parse))
            .OfType<T>();
}
