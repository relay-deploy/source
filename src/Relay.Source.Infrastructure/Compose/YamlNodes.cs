namespace Relay.Source.Infrastructure.Compose;

internal static class YamlNodes
{
    public static IReadOnlyDictionary<string, object?> AsMap(this object? node) =>
        node is IDictionary<object, object?> map
            ? map.ToDictionary(pair => pair.Key.ToString() ?? string.Empty, pair => pair.Value)
            : new Dictionary<string, object?>();

    public static IReadOnlyList<object?> AsList(this object? node) =>
        node is IList<object?> list ? [.. list] : [];

    public static string? AsText(this object? node) => node?.ToString();

    public static bool IsTrue(this object? node) =>
        string.Equals(node?.ToString(), "true", StringComparison.OrdinalIgnoreCase);

    public static object? Get(this IReadOnlyDictionary<string, object?> map, string key) => map.GetValueOrDefault(key);
}
