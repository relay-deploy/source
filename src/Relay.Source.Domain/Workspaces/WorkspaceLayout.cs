using Relay.Source.Domain.Modules;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Workspaces;

public sealed record WorkspaceLayout(IReadOnlyList<Workspace> Workspaces)
{
    public bool IsMember(string path) => Workspaces.Any(workspace => workspace.Contains(path));

    public bool IsContainerRoot(string path) => Workspaces.Any(workspace => workspace.IsContainer && workspace.Root == path);

    public bool IsEmptyContainer(ModuleCandidate module, IEnumerable<ModuleCandidate> modules) =>
        IsContainerRoot(module.Path)
        && modules.Any(other => other.Path != module.Path && other.Path.IsUnder(module.Path));
}
