using System.Text.RegularExpressions;
using System.Xml;
using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Manifests;

public sealed partial record SolutionManifest(IReadOnlyList<string> ProjectPaths)
{
    public static Result<SolutionManifest> Parse(string path, string content)
    {
        try
        {
            var projects = path.EndsWith(".slnx", StringComparison.Ordinal)
                ? XmlDocuments.Parse(content).Root!.DescendantsNamed("Project").Select(project => project.Attribute("Path")?.Value ?? string.Empty)
                : ProjectLine().Matches(content).Select(match => match.Groups[1].Value);
            return new SolutionManifest(
                [.. projects.Select(project => project.Replace('\\', '/')).Where(IsProjectFile)]);
        }
        catch (XmlException)
        {
            return DetectionErrors.ManifestUnreadable(path);
        }
    }

    private static bool IsProjectFile(string path) =>
        path.EndsWith(".csproj", StringComparison.Ordinal) || path.EndsWith(".fsproj", StringComparison.Ordinal);

    [GeneratedRegex("Project\\(\"\\{[^}]+\\}\"\\)\\s*=\\s*\"[^\"]*\"\\s*,\\s*\"([^\"]+)\"")]
    private static partial Regex ProjectLine();
}
