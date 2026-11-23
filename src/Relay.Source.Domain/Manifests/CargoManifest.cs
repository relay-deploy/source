using System.Text.RegularExpressions;

namespace Relay.Source.Domain.Manifests;

public sealed partial record CargoManifest(bool HasPackage, bool HasWorkspace, bool HasBinaryTarget, IReadOnlyList<string> Members)
{
    public static CargoManifest Parse(string content)
    {
        var section = string.Empty;
        var inMembers = false;
        var hasPackage = false;
        var hasWorkspace = false;
        var hasBinaryTarget = false;
        var members = new List<string>();
        foreach (var line in content.Split('\n').Select(line => line.Split('#')[0].Trim()))
        {
            if (line.StartsWith('['))
            {
                section = line.Trim('[', ']').Trim();
                hasPackage |= section == "package";
                hasWorkspace |= section == "workspace";
                hasBinaryTarget |= section == "bin";
                continue;
            }

            if (section != "workspace")
            {
                continue;
            }

            inMembers |= line.StartsWith("members", StringComparison.Ordinal);
            if (!inMembers)
            {
                continue;
            }

            members.AddRange(Quoted().Matches(line).Select(match => match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value));
            inMembers = !line.Contains(']', StringComparison.Ordinal);
        }

        return new CargoManifest(hasPackage, hasWorkspace, hasBinaryTarget, members);
    }

    [GeneratedRegex("\"([^\"]*)\"|'([^']*)'")]
    private static partial Regex Quoted();
}
