using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Compose;

public sealed record ComposeTranslation(
    IReadOnlyList<ComposeCandidate> Candidates,
    IReadOnlyList<ComposeRejection> Rejections,
    IReadOnlyList<DetectionNote> Notes);
