using System.Text.RegularExpressions;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Manifests;

public sealed partial record PythonManifest(IReadOnlySet<string> Dependencies)
{
    public bool Declares(string name) => Dependencies.Contains(name);

    public static PythonManifest Parse(string path, string content)
    {
        var names = path.FileNameOf() == "requirements.txt" ? FromRequirements(content) : FromToml(content);
        return new PythonManifest(names.Select(Normalize).ToHashSet());
    }

    private static IEnumerable<string> FromRequirements(string content) =>
        content.Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && line[0] != '#' && line[0] != '-')
            .Select(line => Requirement().Match(line))
            .Where(match => match.Success)
            .Select(match => match.Value);

    private static IEnumerable<string> FromToml(string content)
    {
        var section = string.Empty;
        var inArray = false;
        foreach (var line in content.Split('\n').Select(line => line.Split('#')[0].Trim()))
        {
            if (line.StartsWith('['))
            {
                section = line.Trim('[', ']').Trim();
                inArray = false;
                continue;
            }

            if (IsDependencyTable(section))
            {
                var key = line.Split('=')[0].Trim().Trim('"');
                if (line.Contains('=', StringComparison.Ordinal) && key != "python")
                {
                    yield return key;
                }

                continue;
            }

            inArray |= DependencyArrayStart().IsMatch(line);
            if (!inArray)
            {
                continue;
            }

            foreach (Match quoted in Quoted().Matches(line))
            {
                var requirement = Requirement().Match(quoted.Groups[1].Value);
                if (requirement.Success)
                {
                    yield return requirement.Value;
                }
            }

            inArray = !line.Contains(']', StringComparison.Ordinal);
        }
    }

    private static bool IsDependencyTable(string section) =>
        section is "packages" or "dev-packages"
        || section.EndsWith("dependencies", StringComparison.Ordinal) && section.StartsWith("tool.poetry", StringComparison.Ordinal);

    private static string Normalize(string name) => name.ToLowerInvariant().Replace('_', '-');

    [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9._-]*")]
    private static partial Regex Requirement();

    [GeneratedRegex("""^(dependencies|install_requires)\s*=\s*\[""")]
    private static partial Regex DependencyArrayStart();

    [GeneratedRegex("\"([^\"]*)\"")]
    private static partial Regex Quoted();
}
