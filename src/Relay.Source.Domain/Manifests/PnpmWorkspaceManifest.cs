namespace Relay.Source.Domain.Manifests;

public sealed record PnpmWorkspaceManifest(IReadOnlyList<string> Packages)
{
    public static PnpmWorkspaceManifest Parse(string content)
    {
        var packages = new List<string>();
        var inPackages = false;
        foreach (var line in content.Split('\n').Select(line => line.TrimEnd()))
        {
            if (line.StartsWith("packages:", StringComparison.Ordinal))
            {
                inPackages = true;
            }
            else if (inPackages && line.TrimStart().StartsWith('-'))
            {
                packages.Add(line.TrimStart()[1..].Trim().Trim('"', '\''));
            }
            else if (inPackages && line.Length > 0 && !char.IsWhiteSpace(line[0]) && line[0] != '#')
            {
                inPackages = false;
            }
        }

        return new PnpmWorkspaceManifest(packages);
    }
}
