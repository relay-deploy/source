using System.Text.Json;
using Relay.Chassis.Primitives;
using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Manifests;

public sealed record AngularManifest(string OutputDirectory)
{
    private const string DefaultOutput = "dist";

    public static Result<AngularManifest> Parse(string path, string content)
    {
        try
        {
            using var document = JsonDocument.Parse(content, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
            var root = document.RootElement;
            var project = root.TryGetProperty("defaultProject", out var configured) && configured.ValueKind == JsonValueKind.String
                ? configured.GetString()
                : root.Entries("projects").Select(entry => entry.Name).FirstOrDefault();
            return new AngularManifest(project is null ? DefaultOutput : $"{DefaultOutput}/{project}");
        }
        catch (JsonException)
        {
            return DetectionErrors.ManifestUnreadable(path);
        }
    }
}
