namespace Relay.Source.Domain.Compose;

public enum ComposeVolumeKind
{
    Anonymous,
    Named,
    SourceMount,
    HostPath,
    DockerSocket,
}
