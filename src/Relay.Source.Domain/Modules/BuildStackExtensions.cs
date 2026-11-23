namespace Relay.Source.Domain.Modules;

public static class BuildStackExtensions
{
    public static string? Buildpack(this BuildStack stack) => stack switch
    {
        BuildStack.Node => "paketo-buildpacks/nodejs",
        BuildStack.Static => "paketo-buildpacks/web-servers",
        BuildStack.Java => "paketo-buildpacks/java",
        BuildStack.Go => "paketo-buildpacks/go",
        BuildStack.Python => "paketo-buildpacks/python",
        BuildStack.Dotnet => "paketo-buildpacks/dotnet-core",
        BuildStack.Rust => "paketocommunity/rust",
        _ => null,
    };

    public static int? FallbackPort(this BuildStack stack) => stack switch
    {
        BuildStack.Node => 3000,
        BuildStack.Python => 8000,
        BuildStack.Custom => null,
        _ => 8080,
    };
}
