namespace Relay.Source.Domain.Compose;

public sealed record ComposeDocument(string Path, IReadOnlyList<ComposeServiceDefinition> Services)
{
    public ComposeDocument OverlayedWith(ComposeDocument overlay)
    {
        var overlaid = Services.Select(service => overlay.Services.FirstOrDefault(other => other.Name == service.Name) is { } match
            ? service.OverlayedWith(match)
            : service);
        var added = overlay.Services.Where(service => Services.All(existing => existing.Name != service.Name));
        return this with { Services = [.. overlaid, .. added] };
    }
}
