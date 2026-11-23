namespace Relay.Source.Domain.Repositories;

public static class RepositoryPaths
{
    private static readonly HashSet<string> ignoredSegments =
    [
        "node_modules",
        ".git",
        ".github",
        "vendor",
        "dist",
        "build",
        "target",
        "__pycache__",
    ];

    public static string DirectoryOf(this string path)
    {
        var index = path.LastIndexOf('/');
        return index < 0 ? string.Empty : path[..index];
    }

    public static string FileNameOf(this string path) => path[(path.LastIndexOf('/') + 1)..];

    public static int DepthOf(this string path) => path.Count(character => character == '/');

    public static bool IsIgnored(this string path) => path.Split('/').Any(ignoredSegments.Contains);

    public static bool HasSegment(this string path, string segment) => path.Split('/').Contains(segment);

    public static bool HasSegmentPair(this string path, string first, string second) =>
        path.Split('/').Zip(path.Split('/').Skip(1)).Any(pair => pair.First == first && pair.Second == second);

    public static bool IsUnder(this string path, string directory) =>
        directory.Length == 0 || path.StartsWith(directory + "/", StringComparison.Ordinal);

    public static string? Join(this string directory, string relative)
    {
        var segments = new List<string>(directory.Split('/', StringSplitOptions.RemoveEmptyEntries));
        foreach (var segment in relative.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".")
            {
                continue;
            }

            if (segment != "..")
            {
                segments.Add(segment);
                continue;
            }

            if (segments.Count == 0)
            {
                return null;
            }

            segments.RemoveAt(segments.Count - 1);
        }

        return string.Join('/', segments);
    }
}
