using Relay.Source.Domain.Compose;

namespace Relay.Source.Infrastructure.Compose;

internal static class ComposePortMapping
{
    public static IEnumerable<ComposePort> ToPorts(this object? node) =>
        node.AsList().Select(ToPort).OfType<ComposePort>();

    private static ComposePort? ToPort(object? entry)
    {
        var map = entry.AsMap();
        if (map.Count > 0)
        {
            return int.TryParse(map.Get("target").AsText(), out var target)
                ? new ComposePort(map.Get("published").AsText(), target)
                : null;
        }

        var parts = (entry.AsText() ?? string.Empty).Split('/')[0].SplitOutsideVariables(':');
        return int.TryParse(parts[^1].Split('-')[0], out var container)
            ? new ComposePort(parts.Count > 1 ? string.Join(':', parts.Take(parts.Count - 1)) : null, container)
            : null;
    }
}
