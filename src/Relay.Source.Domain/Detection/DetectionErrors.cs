using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Detection;

public static class DetectionErrors
{
    public static Error ManifestUnreadable(string path) =>
        Error.Validation("detection.manifest_unreadable", $"{path} could not be parsed, so it was ignored.");

    public static Error FileUnreadable(string path) =>
        Error.NotFound("detection.file_unreadable", $"{path} could not be read.");
}
