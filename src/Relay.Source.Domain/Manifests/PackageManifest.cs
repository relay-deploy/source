using System.Text.Json;
using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Manifests;

public sealed record PackageManifest(
    IReadOnlySet<string> Dependencies,
    IReadOnlySet<string> Scripts,
    IReadOnlyList<string> Workspaces,
    bool UsesWorkspaceProtocol)
{
    private static readonly string[] dependencySections =
        ["dependencies", "devDependencies", "peerDependencies", "optionalDependencies"];

    private static readonly JsonDocumentOptions options = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    public bool Declares(params string[] names) => names.Any(Dependencies.Contains);

    public bool DeclaresWorkspaces => Workspaces.Count > 0;

    public static Result<PackageManifest> Parse(string path, string content)
    {
        try
        {
            using var document = JsonDocument.Parse(content, options);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return DetectionErrors.ManifestUnreadable(path);
            }

            var dependencies = dependencySections.SelectMany(section => root.Entries(section)).ToList();
            return new PackageManifest(
                dependencies.Select(entry => entry.Name).ToHashSet(),
                root.Entries("scripts").Select(entry => entry.Name).ToHashSet(),
                root.WorkspacePatterns(),
                dependencies.Any(entry => entry.Value.ValueKind == JsonValueKind.String
                    && entry.Value.GetString()!.StartsWith("workspace:", StringComparison.Ordinal)));
        }
        catch (JsonException)
        {
            return DetectionErrors.ManifestUnreadable(path);
        }
    }
}
