using Relay.Source.Domain.Compose;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Application.Detection;

public sealed record LoadedRepository(
    IReadOnlyList<RepositoryFile> Files,
    IReadOnlyList<ComposeDocument> ComposeDocuments,
    IReadOnlyList<DetectionNote> Notes);
