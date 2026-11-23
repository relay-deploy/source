using Relay.Source.Domain.Manifests;

namespace Relay.Source.Domain.Modules;

public static class NodeModules
{
    private const string DefaultOutput = "dist";

    public static DirectoryDetection? From(DirectoryManifests directory)
    {
        if (directory.Package is not { } package)
        {
            return null;
        }

        var module = package switch
        {
            _ when package.Declares("next") || directory.HasFilePrefixed("next.config.") =>
                ModuleCandidate.In(directory, BuildStack.Node, Framework.NextJs),
            _ when package.Declares("@angular/core") || directory.HasFile("angular.json") =>
                StaticModule(directory, Framework.Angular, directory.Angular?.OutputDirectory ?? DefaultOutput),
            _ when package.Declares("vue") => StaticModule(directory, Framework.Vue, OutputOf(package)),
            _ when package.Declares("react") => StaticModule(directory, Framework.React, OutputOf(package)),
            _ when package.Declares("express", "fastify", "@nestjs/core") =>
                ModuleCandidate.In(directory, BuildStack.Node, Framework.Express),
            _ when package.Scripts.Contains("start") => ModuleCandidate.In(directory, BuildStack.Node, null),
            _ => StaticModule(directory, null, OutputOf(package)),
        };
        return DirectoryDetection.Found(module);
    }

    private static ModuleCandidate StaticModule(DirectoryManifests directory, Framework? framework, string output) =>
        ModuleCandidate.In(directory, BuildStack.Static, framework) with { WebServerRoot = output };

    private static string OutputOf(PackageManifest package) => package.Declares("react-scripts") ? "build" : DefaultOutput;
}
