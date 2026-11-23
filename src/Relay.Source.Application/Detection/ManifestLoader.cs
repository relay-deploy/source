using Relay.Chassis.Primitives;
using Relay.Source.Application.Repositories;
using Relay.Source.Domain.Compose;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Application.Detection;

public sealed class ManifestLoader(IRepositoryFiles files, IComposeParser composeParser)
{
    public const int MaxManifests = 60;

    private const int MaxParallelReads = 8;

    public async Task<Result<LoadedRepository>> Load(RepositoryCommit commit, RepositoryTree tree, CancellationToken cancellationToken)
    {
        var notes = new List<DetectionNote>();
        if (tree.Truncated)
        {
            notes.Add(DetectionNotes.TreeTruncated());
        }

        var manifestPaths = tree.Paths.Where(ManifestFiles.IsManifest).OrderBy(RepositoryPaths.DepthOf).ThenBy(path => path, StringComparer.Ordinal).ToList();
        if (manifestPaths.Count > MaxManifests)
        {
            notes.Add(DetectionNotes.ManifestLimitReached(MaxManifests));
        }

        var composePaths = tree.Paths.Where(ManifestFiles.IsCompose).Order(StringComparer.Ordinal).ToList();
        var reads = await ReadAll(commit, [.. manifestPaths.Take(MaxManifests), .. composePaths], cancellationToken);
        var failed = reads.FirstOrDefault(read => read.Content.IsFailure && read.Content.Error.Kind != ErrorKind.NotFound);
        if (failed is not null)
        {
            return failed.Content.Error;
        }

        notes.AddRange(reads.Where(read => read.Content.IsFailure).Select(read => DetectionNote.From(DetectionErrors.FileUnreadable(read.Path), read.Path)));
        var readable = reads.Where(read => read.Content.IsSuccess).Select(read => new RepositoryFile(read.Path, read.Content.Value)).ToList();
        var documents = new List<ComposeDocument>();
        foreach (var file in readable.Where(file => file.Path.IsCompose()))
        {
            var parsed = composeParser.Parse(file.Path, file.Content);
            if (parsed.IsSuccess)
            {
                documents.Add(parsed.Value);
            }
            else
            {
                notes.Add(DetectionNote.From(parsed.Error, file.Path));
            }
        }

        return new LoadedRepository(readable, documents, notes);
    }

    private async Task<IReadOnlyList<FileRead>> ReadAll(RepositoryCommit commit, IReadOnlyList<string> paths, CancellationToken cancellationToken)
    {
        using var limit = new SemaphoreSlim(MaxParallelReads);
        var reads = paths.Select(async path =>
        {
            await limit.WaitAsync(cancellationToken);
            try
            {
                return new FileRead(path, await files.ReadText(commit, path, cancellationToken));
            }
            finally
            {
                limit.Release();
            }
        });
        return await Task.WhenAll(reads);
    }
}
