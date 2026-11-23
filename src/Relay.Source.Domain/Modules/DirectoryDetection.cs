using Relay.Source.Domain.Detection;

namespace Relay.Source.Domain.Modules;

public sealed record DirectoryDetection(ModuleCandidate? Module, DetectionNote? Note)
{
    public static DirectoryDetection Found(ModuleCandidate module) => new(module, null);

    public static DirectoryDetection Skipped(DetectionNote note) => new(null, note);
}
