using Relay.Source.Domain.Databases;
using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Modules;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Compose;

public static class ComposeTranslator
{
    public static ComposeTranslation Translate(
        IEnumerable<ComposeDocument> documents,
        RepositorySnapshot snapshot,
        ManifestSet manifests)
    {
        var translations = ComposeFiles.Merge(documents)
            .SelectMany(document => document.Services)
            .Select(service => Translate(service, snapshot, manifests))
            .ToList();
        return new ComposeTranslation(
            [.. translations.Select(translation => translation.Candidate).OfType<ComposeCandidate>()],
            [.. translations.SelectMany(translation => translation.Rejections)],
            [.. translations.SelectMany(translation => translation.Notes)]);
    }

    private static ServiceTranslation Translate(ComposeService service, RepositorySnapshot snapshot, ManifestSet manifests)
    {
        var violations = ComposeViolations.Of(service).ToList();
        if (violations.Count > 0)
        {
            return ServiceTranslation.Rejected(
                violations.Select(violation => new ComposeRejection(service.Name, service.File, violation.Key, violation.Error)));
        }

        var candidate = new ComposeCandidate(
            service.Name,
            ComposeKind.Image,
            service.File,
            service.Image,
            null,
            null,
            null,
            null,
            [.. service.Ports.Select(port => port.Target).Distinct()],
            [.. service.Volumes.Where(volume => volume.Kind() == ComposeVolumeKind.Named).Select(volume => volume.Source!)],
            service.Environment,
            service.EnvFiles);
        return ServiceTranslation.Accepted(Classify(service, candidate, snapshot, manifests), NotesOf(service));
    }

    private static ComposeCandidate Classify(ComposeService service, ComposeCandidate candidate, RepositorySnapshot snapshot, ManifestSet manifests)
    {
        if (service.Build is not null)
        {
            var context = ComposeViolations.BuildContextOf(service)!;
            var dockerfile = context.Join(service.Build.Dockerfile ?? "Dockerfile")!;
            return candidate with
            {
                Kind = ComposeKind.Build,
                Module = ModuleOf(context, dockerfile.DirectoryOf(), snapshot, manifests),
                BuildContext = context,
                Dockerfile = dockerfile,
            };
        }

        return DatabaseEngines.FromImage(service.Image!) is { } engine
            ? candidate with
            {
                Kind = ComposeKind.Database,
                Database = DatabaseSuggestion.FromComposeService(service.Name, service.Image!, engine),
            }
            : candidate;
    }

    private static ModuleCandidate ModuleOf(string context, string dockerfileDirectory, RepositorySnapshot snapshot, ManifestSet manifests)
    {
        var detected = ModuleDetector.DetectIn(dockerfileDirectory, snapshot, manifests)
            ?? ModuleDetector.DetectIn(context, snapshot, manifests)
            ?? ModuleCandidate.For(context, BuildStack.Custom, null, BuildMode.Dockerfile) with { Confidence = 0.6 };
        return detected with { Mode = BuildMode.Dockerfile };
    }

    private static IEnumerable<DetectionNote> NotesOf(ComposeService service)
    {
        foreach (var port in service.Ports.Where(port => port.Published is not null))
        {
            yield return DetectionNotes.ComposeHostPortDropped(service.Name, port.ToString());
        }

        foreach (var volume in service.Volumes.Where(volume => volume.Kind() == ComposeVolumeKind.SourceMount))
        {
            yield return DetectionNotes.ComposeSourceMountDropped(service.Name, volume.ToString());
        }
    }
}
