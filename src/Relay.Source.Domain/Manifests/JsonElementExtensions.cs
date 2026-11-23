using System.Text.Json;

namespace Relay.Source.Domain.Manifests;

internal static class JsonElementExtensions
{
    public static IEnumerable<JsonProperty> Entries(this JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Object
            ? value.EnumerateObject()
            : [];

    public static IReadOnlyList<string> Strings(this JsonElement element) =>
        element.ValueKind == JsonValueKind.Array
            ? [.. element.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.String).Select(item => item.GetString()!)]
            : [];

    public static IReadOnlyList<string> WorkspacePatterns(this JsonElement root)
    {
        if (!root.TryGetProperty("workspaces", out var workspaces))
        {
            return [];
        }

        return workspaces.ValueKind == JsonValueKind.Object && workspaces.TryGetProperty("packages", out var packages)
            ? packages.Strings()
            : workspaces.Strings();
    }
}
