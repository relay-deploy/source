using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Workspaces;

public sealed record Workspace(WorkspaceKind Kind, string Root, IReadOnlyList<string> Members, bool IsContainer)
{
    public bool Contains(string path) =>
        path != Root && path.IsUnder(Root) && Members.Any(member => PathGlob.IsMatch(member, path));
}
