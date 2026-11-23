namespace Relay.Source.Domain.Compose;

public sealed record ComposePort(string? Published, int Target)
{
    public override string ToString() => Published is null ? $"{Target}" : $"{Published}:{Target}";
}
