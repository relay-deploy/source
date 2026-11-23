namespace Relay.Source.Domain.Modules;

public static class ModuleSelectors
{
    public const string PathScope = "--path";

    public static ModuleCandidate WithSelector(this ModuleCandidate module) =>
        module with { Selector = module.SelectorFor() };

    public static string? RepositoryRelativeWebServerRoot(this ModuleCandidate module) =>
        module.WebServerRoot switch
        {
            null => null,
            "." => module.Path,
            var root => module.Path.Length == 0 ? root : $"{module.Path}/{root}",
        };

    private static BuildSelector? SelectorFor(this ModuleCandidate module)
    {
        if (!module.WorkspaceBuild || module.Path.Length == 0)
        {
            return null;
        }

        return module.Stack switch
        {
            BuildStack.Java => new BuildSelector(
                module.Tool == BuildTool.Gradle ? "BP_GRADLE_BUILT_MODULE" : "BP_MAVEN_BUILT_MODULE",
                module.Path),
            BuildStack.Node or BuildStack.Static => new BuildSelector("BP_NODE_PROJECT_PATH", module.Path),
            BuildStack.Go => new BuildSelector("BP_GO_TARGETS", $"./{module.Path}"),
            BuildStack.Dotnet => new BuildSelector("BP_DOTNET_PROJECT_PATH", module.Path),
            _ => new BuildSelector(PathScope, module.Path),
        };
    }
}
