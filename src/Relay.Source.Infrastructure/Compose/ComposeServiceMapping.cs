using Relay.Source.Domain.Compose;

namespace Relay.Source.Infrastructure.Compose;

internal static class ComposeServiceMapping
{
    public static ComposeServiceDefinition ToService(string file, string name, object? node)
    {
        var map = node.AsMap();
        return new ComposeServiceDefinition(
            name,
            file,
            map.Get("image").AsText(),
            map.ContainsKey("build") ? ToBuild(map.Get("build")) : null,
            [.. map.Get("ports").ToPorts()],
            [.. map.Get("volumes").ToVolumes()],
            map.Get("environment").ToEnvironment(),
            [.. map.Get("env_file").ToEnvFiles()],
            map.Get("privileged").IsTrue(),
            map.Get("network_mode").AsText(),
            [.. map.Get("devices").AsList().Select(device => device.AsText()).OfType<string>()]);
    }

    private static ComposeBuild ToBuild(object? node)
    {
        if (node is string context)
        {
            return new ComposeBuild(context, null);
        }

        var map = node.AsMap();
        return new ComposeBuild(map.Get("context").AsText(), map.Get("dockerfile").AsText());
    }
}
