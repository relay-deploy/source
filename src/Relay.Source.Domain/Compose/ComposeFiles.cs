using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Compose;

public static class ComposeFiles
{
    private static readonly string[] baseNames = ["compose.yaml", "compose.yml", "docker-compose.yaml", "docker-compose.yml"];

    public static bool IsBase(string path) => baseNames.Contains(path.FileNameOf());

    public static IReadOnlyList<ComposeDocument> Merge(IEnumerable<ComposeDocument> documents) =>
        [.. documents.GroupBy(document => document.Path.DirectoryOf()).SelectMany(MergeDirectory)];

    private static IEnumerable<ComposeDocument> MergeDirectory(IGrouping<string, ComposeDocument> directory)
    {
        var ordered = directory.OrderBy(document => BasePriority(document.Path)).ThenBy(document => document.Path, StringComparer.Ordinal).ToList();
        var hasBase = ordered.Any(document => IsBase(document.Path));
        if (!hasBase)
        {
            return ordered;
        }

        return [ordered.Skip(1).Aggregate(ordered[0], (merged, overlay) => merged.OverlayedWith(overlay))];
    }

    private static int BasePriority(string path) =>
        Array.IndexOf(baseNames, path.FileNameOf()) is var index and >= 0 ? index : baseNames.Length;
}
