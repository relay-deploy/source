using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Workspaces;

public static class WorkspaceDiscovery
{
    public static WorkspaceLayout Find(ManifestSet manifests) =>
        new([.. manifests.Directories.SelectMany(From)]);

    private static IEnumerable<Workspace> From(DirectoryManifests directory)
    {
        var root = directory.Directory;
        if (directory.Pnpm is { } pnpm)
        {
            yield return new Workspace(WorkspaceKind.Pnpm, root, Resolve(root, pnpm.Packages), true);
        }
        else if (directory.Package is { DeclaresWorkspaces: true } package)
        {
            var kind = directory.HasFile("yarn.lock") ? WorkspaceKind.Yarn : WorkspaceKind.Npm;
            yield return new Workspace(kind, root, Resolve(root, package.Workspaces), true);
        }

        if (directory.Jvm is { DeclaresModules: true } jvm)
        {
            yield return new Workspace(WorkspaceKind.Maven, root, Resolve(root, jvm.Modules.Select(TrimPom)), true);
        }

        foreach (var solution in directory.Solutions)
        {
            var members = solution.ProjectPaths.Select(project => project.DirectoryOf());
            yield return new Workspace(WorkspaceKind.Dotnet, root, Resolve(root, members), false);
        }

        if (directory.GoWork is { } goWork)
        {
            yield return new Workspace(WorkspaceKind.Go, root, Resolve(root, goWork.Directories), false);
        }

        if (directory.Cargo is { HasWorkspace: true } cargo)
        {
            yield return new Workspace(WorkspaceKind.Cargo, root, Resolve(root, cargo.Members), false);
        }
    }

    private static string TrimPom(string module) =>
        module.EndsWith("/pom.xml", StringComparison.Ordinal) ? module[..^"/pom.xml".Length] : module;

    private static IReadOnlyList<string> Resolve(string root, IEnumerable<string> patterns) =>
        [.. patterns.Select(pattern => root.Join(pattern)).OfType<string>().Where(pattern => pattern.Length > 0)];
}
