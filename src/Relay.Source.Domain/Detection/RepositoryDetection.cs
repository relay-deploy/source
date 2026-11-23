using Relay.Source.Domain.Compose;
using Relay.Source.Domain.Databases;
using Relay.Source.Domain.Modules;

namespace Relay.Source.Domain.Detection;

public sealed record RepositoryDetection(
    IReadOnlyList<ModuleCandidate> Modules,
    IReadOnlyList<DatabaseSuggestion> Databases,
    IReadOnlyList<ComposeCandidate> ComposeServices,
    IReadOnlyList<DetectionNote> Notes,
    IReadOnlyList<ComposeRejection> Rejections);
