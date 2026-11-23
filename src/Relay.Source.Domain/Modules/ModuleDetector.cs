using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Repositories;
using Relay.Source.Domain.Workspaces;

namespace Relay.Source.Domain.Modules;

public static class ModuleDetector
{
    private const double ContainerConfidence = 0.5;

    private static readonly Func<DirectoryManifests, RepositorySnapshot, DirectoryDetection?>[] ecosystems =
    [
        (directory, _) => JvmModules.From(directory),
        (directory, _) => GoModules.From(directory),
        (directory, _) => DotnetModules.From(directory),
        RustModules.From,
        (directory, _) => PythonModules.From(directory),
        (directory, _) => NodeModules.From(directory),
        (directory, _) => DockerfileModules.From(directory),
    ];

    public static ModuleDetection Detect(RepositorySnapshot snapshot, ManifestSet manifests)
    {
        var detections = manifests.Directories
            .OrderBy(directory => directory.Directory, StringComparer.Ordinal)
            .Select(directory => DetectDirectory(directory, snapshot))
            .OfType<DirectoryDetection>()
            .ToList();
        var notes = detections.Select(detection => detection.Note).OfType<DetectionNote>().ToList();
        var modules = detections.Select(detection => detection.Module).OfType<ModuleCandidate>().ToList();
        if (modules.Count == 0)
        {
            return new ModuleDetection(StaticSiteModules.From(snapshot) is { } site ? [site] : [], notes);
        }

        var layout = WorkspaceDiscovery.Find(manifests);
        var placed = modules
            .Where(module => !layout.IsEmptyContainer(module, modules))
            .Select(module => Place(module, layout, manifests))
            .ToList();
        return new ModuleDetection(placed, notes);
    }

    public static ModuleCandidate? DetectIn(string directory, RepositorySnapshot snapshot, ManifestSet manifests) =>
        manifests.At(directory) is { } found ? DetectDirectory(found, snapshot)?.Module : null;

    private static DirectoryDetection? DetectDirectory(DirectoryManifests directory, RepositorySnapshot snapshot)
    {
        DirectoryDetection? skipped = null;
        foreach (var ecosystem in ecosystems)
        {
            var detection = ecosystem(directory, snapshot);
            if (detection?.Module is not null)
            {
                return detection;
            }

            skipped ??= detection;
        }

        return skipped;
    }

    private static ModuleCandidate Place(ModuleCandidate module, WorkspaceLayout layout, ManifestSet manifests)
    {
        var usesWorkspaceProtocol = manifests.At(module.Path)?.Package?.UsesWorkspaceProtocol ?? false;
        var placed = module with
        {
            WorkspaceBuild = layout.IsMember(module.Path) || usesWorkspaceProtocol,
            Confidence = layout.IsContainerRoot(module.Path) ? ContainerConfidence : module.Confidence,
        };
        return placed.WithSelector();
    }
}
