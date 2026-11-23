namespace Relay.Source.Domain.Compose;

public static class ComposeVolumes
{
    private static readonly char[] absolutePrefixes = ['/', '~', '$'];

    public static ComposeVolumeKind Kind(this ComposeVolume volume)
    {
        if (volume.Source is null || volume.Type == "tmpfs")
        {
            return ComposeVolumeKind.Anonymous;
        }

        if (IsDockerSocket(volume))
        {
            return ComposeVolumeKind.DockerSocket;
        }

        if (absolutePrefixes.Contains(volume.Source[0]))
        {
            return ComposeVolumeKind.HostPath;
        }

        return volume.Source[0] == '.' || volume.Type == "bind"
            ? ComposeVolumeKind.SourceMount
            : ComposeVolumeKind.Named;
    }

    private static bool IsDockerSocket(ComposeVolume volume) =>
        volume.Source!.Contains("docker.sock", StringComparison.Ordinal) || volume.Target.EndsWith("docker.sock", StringComparison.Ordinal);
}
