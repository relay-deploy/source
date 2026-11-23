using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Modules;

public static class ModuleExposure
{
    private static readonly HashSet<string> workerSegments = ["worker", "workers", "jobs", "cron", "consumer"];

    public static bool IsExposedByDefault(string path) => !workerSegments.Contains(path.FileNameOf());
}
