using Relay.Chassis.Primitives;
using Relay.Source.Application.Repositories;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Application.Detection;

public sealed class DetectRepository(IRepositoryFiles files, ManifestLoader loader)
{
    public async Task<Result<RepositoryDetection>> Handle(DetectRepositoryQuery query, CancellationToken cancellationToken)
    {
        var commit = RepositoryCommit.Create(query.Owner, query.Repository, query.Reference);
        if (commit.IsFailure)
        {
            return commit.Error;
        }

        var tree = await files.ListFiles(commit.Value, cancellationToken);
        if (tree.IsFailure)
        {
            return tree.Error;
        }

        var loaded = await loader.Load(commit.Value, tree.Value, cancellationToken);
        if (loaded.IsFailure)
        {
            return loaded.Error;
        }

        var snapshot = new RepositorySnapshot(tree.Value.Paths, loaded.Value.Files);
        return RepositoryDetector.Detect(snapshot, loaded.Value.ComposeDocuments, loaded.Value.Notes);
    }
}
