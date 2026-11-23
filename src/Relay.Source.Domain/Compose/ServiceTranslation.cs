using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Compose;

public sealed record ServiceTranslation(
    ComposeCandidate? Candidate,
    IReadOnlyList<ComposeRejection> Rejections,
    IReadOnlyList<DetectionNote> Notes)
{
    public static ServiceTranslation Rejected(IEnumerable<ComposeRejection> rejections) => new(null, [.. rejections], []);

    public static ServiceTranslation Accepted(ComposeCandidate candidate, IEnumerable<DetectionNote> notes) => new(candidate, [], [.. notes]);
}
