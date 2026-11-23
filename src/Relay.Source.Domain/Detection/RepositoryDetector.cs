using Relay.Source.Domain.Compose;
using Relay.Source.Domain.Databases;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Modules;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Detection;

public static class RepositoryDetector
{
    public static RepositoryDetection Detect(
        RepositorySnapshot snapshot,
        IEnumerable<ComposeDocument> composeDocuments,
        IEnumerable<DetectionNote> readNotes)
    {
        var manifests = ManifestSet.From(snapshot);
        var modules = ModuleDetector.Detect(snapshot, manifests);
        var compose = ComposeTranslator.Translate(composeDocuments, snapshot, manifests);
        var databases = new[] { DatabaseDetector.Detect(snapshot) }
            .OfType<DatabaseSuggestion>()
            .Concat(compose.Candidates.Select(candidate => candidate.Database).OfType<DatabaseSuggestion>())
            .DistinctBy(suggestion => (suggestion.Engine, suggestion.ComposeService))
            .ToList();
        return new RepositoryDetection(
            modules.Modules,
            databases,
            compose.Candidates,
            [.. readNotes, .. manifests.Notes, .. modules.Notes, .. compose.Notes],
            compose.Rejections);
    }
}
