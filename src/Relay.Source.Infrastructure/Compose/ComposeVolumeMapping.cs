using Relay.Source.Domain.Compose;

namespace Relay.Source.Infrastructure.Compose;

internal static class ComposeVolumeMapping
{
    public static IEnumerable<ComposeVolume> ToVolumes(this object? node) =>
        node.AsList().Select(ToVolume).OfType<ComposeVolume>();

    private static ComposeVolume? ToVolume(object? entry)
    {
        var map = entry.AsMap();
        if (map.Count > 0)
        {
            return map.Get("target").AsText() is { } target
                ? new ComposeVolume(map.Get("type").AsText(), map.Get("source").AsText(), target)
                : null;
        }

        var parts = (entry.AsText() ?? string.Empty).SplitOutsideVariables(':');
        return parts.Count switch
        {
            1 when string.IsNullOrEmpty(parts[0]) => null,
            1 => new ComposeVolume(null, null, parts[0]),
            _ => new ComposeVolume(null, parts[0], parts[1]),
        };
    }
}
