using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Modules;

public sealed record ModuleDetection(IReadOnlyList<ModuleCandidate> Modules, IReadOnlyList<DetectionNote> Notes);
