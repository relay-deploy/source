namespace Relay.Source.Domain.Repositories;

public sealed class RepositorySnapshot(IEnumerable<string> paths, IEnumerable<RepositoryFile> files)
{
    private readonly string[] livePaths = [.. paths.Where(path => !path.IsIgnored())];

    private readonly HashSet<string> pathSet = [.. paths.Where(path => !path.IsIgnored())];

    private readonly ILookup<string, string> namesByDirectory =
        paths.Where(path => !path.IsIgnored()).ToLookup(RepositoryPaths.DirectoryOf, RepositoryPaths.FileNameOf);

    private readonly Dictionary<string, RepositoryFile> filesByPath =
        files.Where(file => !file.Path.IsIgnored()).ToDictionary(file => file.Path);

    public IReadOnlyList<string> Paths => livePaths;

    public IReadOnlyCollection<RepositoryFile> Files => filesByPath.Values;

    public IReadOnlyList<string> NamesIn(string directory) => [.. namesByDirectory[directory]];

    public IReadOnlyList<string> Directories() => [.. namesByDirectory.Select(group => group.Key).Order(StringComparer.Ordinal)];

    public RepositoryFile? FileAt(string path) => filesByPath.GetValueOrDefault(path);

    public bool Contains(string path) => pathSet.Contains(path);
}
