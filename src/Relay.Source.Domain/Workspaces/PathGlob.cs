namespace Relay.Source.Domain.Workspaces;

public static class PathGlob
{
    public static bool IsMatch(string pattern, string path) =>
        Match(pattern.Split('/', StringSplitOptions.RemoveEmptyEntries), path.Split('/', StringSplitOptions.RemoveEmptyEntries));

    private static bool Match(ReadOnlySpan<string> pattern, ReadOnlySpan<string> path)
    {
        if (pattern.IsEmpty)
        {
            return path.IsEmpty;
        }

        if (pattern[0] == "**")
        {
            for (var skipped = 0; skipped <= path.Length; skipped++)
            {
                if (Match(pattern[1..], path[skipped..]))
                {
                    return true;
                }
            }

            return false;
        }

        return !path.IsEmpty && SegmentMatches(pattern[0], path[0]) && Match(pattern[1..], path[1..]);
    }

    private static bool SegmentMatches(string pattern, string segment)
    {
        var parts = pattern.Split('*');
        if (parts.Length == 1)
        {
            return pattern == segment;
        }

        if (!segment.StartsWith(parts[0], StringComparison.Ordinal) || !segment.EndsWith(parts[^1], StringComparison.Ordinal))
        {
            return false;
        }

        var position = parts[0].Length;
        var end = segment.Length - parts[^1].Length;
        foreach (var middle in parts[1..^1])
        {
            var found = segment.IndexOf(middle, position, StringComparison.Ordinal);
            if (found < 0 || found + middle.Length > end)
            {
                return false;
            }

            position = found + middle.Length;
        }

        return position <= end;
    }
}
