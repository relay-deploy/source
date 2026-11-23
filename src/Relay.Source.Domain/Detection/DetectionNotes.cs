namespace Relay.Source.Domain.Detection;

public static class DetectionNotes
{
    public static DetectionNote TreeTruncated() =>
        new("detection.tree_truncated", string.Empty, "The provider truncated the file tree, so some modules may be missing.");

    public static DetectionNote ManifestLimitReached(int limit) =>
        new("detection.manifest_limit_reached", string.Empty, $"Only the {limit} shallowest manifests were read.");

    public static DetectionNote LibrarySkipped(string path, string kind) =>
        new("detection.library_skipped", path, $"{path} is a {kind} library with no entry point, so it is not a deployable module.");

    public static DetectionNote ComposeHostPortDropped(string service, string port) =>
        new("compose.host_port_dropped", service, $"Service '{service}' publishes '{port}' on the host. Host ports are dropped, and relay routes traffic itself.");

    public static DetectionNote ComposeSourceMountDropped(string service, string mount) =>
        new("compose.source_mount_dropped", service, $"Service '{service}' bind-mounts repository files ('{mount}'). The mount is dropped, because the image is built from the repository.");
}
