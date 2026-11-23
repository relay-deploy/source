namespace Relay.Source.Domain.Compose;

public sealed record ComposeServiceDefinition(
    string Name,
    string File,
    string? Image,
    ComposeBuild? Build,
    IReadOnlyList<ComposePort> Ports,
    IReadOnlyList<ComposeVolume> Volumes,
    IReadOnlyDictionary<string, string?> Environment,
    IReadOnlyList<string> EnvFiles,
    bool Privileged,
    string? NetworkMode,
    IReadOnlyList<string> Devices)
{
    public ComposeServiceDefinition OverlayedWith(ComposeServiceDefinition overlay) =>
        this with
        {
            Image = overlay.Image ?? Image,
            Build = overlay.Build is null ? Build : new ComposeBuild(overlay.Build.Context ?? Build?.Context, overlay.Build.Dockerfile ?? Build?.Dockerfile),
            Ports = [.. Ports, .. overlay.Ports],
            Volumes = [.. Volumes.Where(volume => overlay.Volumes.All(other => other.Target != volume.Target)), .. overlay.Volumes],
            Environment = Environment.Concat(overlay.Environment).GroupBy(pair => pair.Key).ToDictionary(group => group.Key, group => group.Last().Value),
            EnvFiles = [.. EnvFiles.Concat(overlay.EnvFiles).Distinct()],
            Privileged = Privileged || overlay.Privileged,
            NetworkMode = overlay.NetworkMode ?? NetworkMode,
            Devices = [.. Devices.Concat(overlay.Devices).Distinct()],
        };
}
