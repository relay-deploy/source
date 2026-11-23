using Relay.Source.Domain.Databases;
using Relay.Source.Domain.Modules;

namespace Relay.Source.Domain.Compose;

public sealed record ComposeCandidate(
    string Service,
    ComposeKind Kind,
    string File,
    string? Image,
    ModuleCandidate? Module,
    string? BuildContext,
    string? Dockerfile,
    DatabaseSuggestion? Database,
    IReadOnlyList<int> ContainerPorts,
    IReadOnlyList<string> NamedVolumes,
    IReadOnlyDictionary<string, string?> Environment,
    IReadOnlyList<string> EnvFiles);
