using System.Text.RegularExpressions;
using Relay.Source.Domain.Manifests;
using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Databases;

public static class DatabaseDetector
{
    private static readonly string[] driverDependencies = ["psycopg2", "postgresql", "pg"];

    private static readonly string[] connectionVariables = ["DATABASE_URL", "SPRING_DATASOURCE_URL"];

    public static DatabaseSuggestion? Detect(RepositorySnapshot snapshot)
    {
        var signals = new List<string>();
        DatabaseEngine? engine = null;

        if (snapshot.Paths.Any(path => path.HasSegment("prisma")))
        {
            signals.Add("prisma directory");
        }

        foreach (var file in snapshot.Files.Where(file => file.Path.IsManifest()).OrderBy(file => file.Path, StringComparer.Ordinal))
        {
            if (file.Content.Contains("@prisma/client", StringComparison.Ordinal))
            {
                signals.Add($"@prisma/client dependency in {file.Path}");
            }

            if (connectionVariables.Any(variable => file.Content.Contains(variable, StringComparison.Ordinal)))
            {
                signals.Add($"database URL in {file.Path}");
            }

            var driver = driverDependencies.FirstOrDefault(name => DriverPattern(name).IsMatch(file.Content));
            if (driver is not null)
            {
                signals.Add($"{driver} driver dependency in {file.Path}");
                engine = DatabaseEngine.Postgres;
            }
        }

        if (snapshot.Paths.Any(path => path.HasSegment("migrations")))
        {
            signals.Add("migrations directory");
        }

        if (snapshot.Paths.Any(path => path.HasSegmentPair("db", "migrate")))
        {
            signals.Add("db/migrate directory");
        }

        return signals.Count == 0
            ? null
            : new DatabaseSuggestion(engine, DatabaseSuggestion.LikelihoodOf(signals.Count), signals, null);
    }

    private static Regex DriverPattern(string name) => new($@"\b{Regex.Escape(name)}\b", RegexOptions.None, TimeSpan.FromSeconds(1));
}
