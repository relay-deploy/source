namespace Relay.Source.Domain.Manifests;

public sealed record GoWorkManifest(IReadOnlyList<string> Directories)
{
    public static GoWorkManifest Parse(string content)
    {
        var directories = new List<string>();
        var inBlock = false;
        foreach (var line in content.Split('\n').Select(line => line.Split("//")[0].Trim()))
        {
            if (inBlock)
            {
                inBlock = line != ")";
                if (inBlock && line.Length > 0)
                {
                    directories.Add(line.Trim('"'));
                }
            }
            else if (line.StartsWith("use", StringComparison.Ordinal))
            {
                var rest = line["use".Length..].Trim();
                inBlock = rest == "(";
                if (!inBlock && rest.Length > 0)
                {
                    directories.Add(rest.Trim('"'));
                }
            }
        }

        return new GoWorkManifest(directories);
    }
}
