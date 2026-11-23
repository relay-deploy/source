using Relay.Source.Domain.Detection;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Modules;

public static class RustModules
{
    public static DirectoryDetection? From(DirectoryManifests directory, RepositorySnapshot snapshot)
    {
        if (directory.Cargo is not { HasPackage: true } cargo)
        {
            return null;
        }

        return cargo.HasBinaryTarget || HasBinaryEntryPoint(directory.Directory, snapshot)
            ? DirectoryDetection.Found(ModuleCandidate.In(directory, BuildStack.Rust, Framework.Rust))
            : DirectoryDetection.Skipped(DetectionNotes.LibrarySkipped(directory.Directory.Length == 0 ? "." : directory.Directory, "Rust"));
    }

    private static bool HasBinaryEntryPoint(string directory, RepositorySnapshot snapshot)
    {
        var source = directory.Join("src")!;
        return snapshot.Contains($"{source}/main.rs") || snapshot.Paths.Any(path => path.IsUnder($"{source}/bin"));
    }
}
