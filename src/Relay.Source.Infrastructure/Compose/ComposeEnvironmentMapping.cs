namespace Relay.Source.Infrastructure.Compose;

internal static class ComposeEnvironmentMapping
{
    public static IReadOnlyDictionary<string, string?> ToEnvironment(this object? node)
    {
        var map = node.AsMap();
        if (map.Count > 0)
        {
            return map.ToDictionary(pair => pair.Key, pair => pair.Value.AsText());
        }

        return node.AsList()
            .Select(entry => entry.AsText() ?? string.Empty)
            .Where(entry => entry.Length > 0)
            .Select(entry => entry.Split('=', 2))
            .GroupBy(parts => parts[0])
            .ToDictionary(group => group.Key, group => group.Last().Length > 1 ? group.Last()[1] : null);
    }

    public static IEnumerable<string> ToEnvFiles(this object? node) =>
        node is string single
            ? [single]
            : node.AsList().Select(entry => entry.AsMap().Count > 0 ? entry.AsMap().Get("path").AsText() : entry.AsText()).OfType<string>();
}
