namespace Relay.Source.Domain.Compose;

public sealed record ComposeVolume(string? Type, string? Source, string Target)
{
    public override string ToString() => Source is null ? Target : $"{Source}:{Target}";
}
