using System.Xml;
using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Modules;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Manifests;

public sealed record JvmManifest(BuildTool Tool, IReadOnlyList<string> Modules, bool UsesSpringBoot)
{
    public bool DeclaresModules => Modules.Count > 0;

    public static Result<JvmManifest> Parse(string path, string content)
    {
        var fileName = path.FileNameOf();
        return fileName == "pom.xml" ? ParsePom(path, content) : ParseGradle(content, fileName);
    }

    private static Result<JvmManifest> ParsePom(string path, string content)
    {
        try
        {
            var root = XmlDocuments.Parse(content).Root!;
            var modules = root.Children("modules").SelectMany(modules => modules.Children("module"))
                .Select(module => module.Value.Trim())
                .Where(module => module.Length > 0)
                .ToList();
            var usesSpringBoot =
                root.DescendantsNamed("artifactId").Any(artifact => artifact.Value.Trim().StartsWith("spring-boot", StringComparison.Ordinal))
                || root.DescendantsNamed("groupId").Any(group => group.Value.Trim() == "org.springframework.boot");
            return new JvmManifest(BuildTool.Maven, modules, usesSpringBoot);
        }
        catch (XmlException)
        {
            return DetectionErrors.ManifestUnreadable(path);
        }
    }

    private static JvmManifest ParseGradle(string content, string fileName) =>
        new(
            BuildTool.Gradle,
            [],
            fileName.StartsWith("build.gradle", StringComparison.Ordinal)
                && (content.Contains("spring-boot-starter", StringComparison.Ordinal)
                    || content.Contains("org.springframework.boot", StringComparison.Ordinal)));
}
