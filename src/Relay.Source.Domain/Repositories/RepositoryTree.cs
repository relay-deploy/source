namespace Relay.Source.Domain.Repositories;

public sealed record RepositoryTree(IReadOnlyList<string> Paths, bool Truncated);
