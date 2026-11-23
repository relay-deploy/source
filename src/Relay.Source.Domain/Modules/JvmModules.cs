using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class JvmModules
{
    public static DirectoryDetection? From(DirectoryManifests directory) =>
        directory.Jvm is not { } jvm
            ? null
            : DirectoryDetection.Found(
                ModuleCandidate.In(directory, BuildStack.Java, jvm.UsesSpringBoot ? Framework.SpringBoot : null) with { Tool = jvm.Tool });
}
