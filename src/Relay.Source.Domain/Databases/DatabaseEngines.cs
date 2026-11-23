using Relay.Source.Domain.Repositories;

namespace Relay.Source.Domain.Databases;

public static class DatabaseEngines
{
    public static DatabaseEngine? FromImage(string image) =>
        NameOf(image) switch
        {
            "postgres" => DatabaseEngine.Postgres,
            "mysql" => DatabaseEngine.MySql,
            "mariadb" => DatabaseEngine.MariaDb,
            "redis" => DatabaseEngine.Redis,
            "mongo" => DatabaseEngine.Mongo,
            _ => null,
        };

    private static string NameOf(string image)
    {
        var withoutDigest = image.Split('@')[0];
        return withoutDigest.FileNameOf().Split(':')[0].ToLowerInvariant();
    }
}
