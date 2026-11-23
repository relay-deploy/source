using Relay.Chassis.Primitives;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Application.Repositories;

public interface IRepositoryFiles
{
    Task<Result<RepositoryTree>> ListFiles(RepositoryCommit commit, CancellationToken cancellationToken);

    Task<Result<string>> ReadText(RepositoryCommit commit, string path, CancellationToken cancellationToken);
}
