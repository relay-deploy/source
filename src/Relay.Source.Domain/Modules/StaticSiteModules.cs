using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Modules;

public static class StaticSiteModules
{
    private const string IndexPage = "index.html";

    public static ModuleCandidate? From(RepositorySnapshot snapshot)
    {
        if (snapshot.Contains(IndexPage))
        {
            return PlainSite(string.Empty);
        }

        var directories = snapshot.Paths
            .Where(path => path.FileNameOf() == IndexPage && path.DepthOf() == 1)
            .Select(RepositoryPaths.DirectoryOf)
            .Distinct()
            .ToList();
        return directories.Count == 1 ? PlainSite(directories[0]) : null;
    }

    private static ModuleCandidate PlainSite(string path) =>
        ModuleCandidate.For(path, BuildStack.Static, Framework.StaticPlain, BuildMode.Buildpack) with
        {
            Confidence = 0.7,
            WebServerRoot = ".",
        };
}
