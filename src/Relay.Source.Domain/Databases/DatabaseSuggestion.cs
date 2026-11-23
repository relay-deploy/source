namespace Relay.Source.Domain.Databases;

public sealed record DatabaseSuggestion(
    DatabaseEngine? Engine,
    Likelihood Likelihood,
    IReadOnlyList<string> Signals,
    string? ComposeService)
{
    public static Likelihood LikelihoodOf(int signals) => signals switch
    {
        1 => Likelihood.Likely,
        _ => Likelihood.Certain,
    };

    public static DatabaseSuggestion FromComposeService(string service, string image, DatabaseEngine engine) =>
        new(engine, Likelihood.Certain, [$"compose service '{service}' runs image {image}"], service);
}
