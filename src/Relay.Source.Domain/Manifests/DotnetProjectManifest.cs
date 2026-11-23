using System.Xml;
using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Manifests;

public sealed record DotnetProjectManifest(bool IsDeployable)
{
    private static readonly string[] testPackagePrefixes =
        ["Microsoft.NET.Test.Sdk", "MSTest", "xunit", "NUnit", "TUnit"];

    private static readonly string[] deployableSdks = ["Microsoft.NET.Sdk.Web", "Microsoft.NET.Sdk.Worker"];

    public static Result<DotnetProjectManifest> Parse(string path, string content)
    {
        try
        {
            var root = XmlDocuments.Parse(content).Root!;
            var sdk = root.Attribute("Sdk")?.Value ?? string.Empty;
            var isExecutable =
                deployableSdks.Any(deployable => sdk.StartsWith(deployable, StringComparison.Ordinal))
                || root.DescendantsNamed("OutputType").Any(type => type.Value.Trim() == "Exe");
            var isTestProject =
                root.DescendantsNamed("IsTestProject").Any(flag => flag.Value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                || root.DescendantsNamed("PackageReference").Any(package =>
                    testPackagePrefixes.Any(prefix => (package.Attribute("Include")?.Value ?? string.Empty).StartsWith(prefix, StringComparison.Ordinal)));
            return new DotnetProjectManifest(isExecutable && !isTestProject);
        }
        catch (XmlException)
        {
            return DetectionErrors.ManifestUnreadable(path);
        }
    }
}
