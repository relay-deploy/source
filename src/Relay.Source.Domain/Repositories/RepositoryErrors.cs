using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Repositories;

public static class RepositoryErrors
{
    public static Error InvalidOwner(string owner) => Error.Validation("repository.invalid_owner", $"'{owner}' is not a valid repository owner.");

    public static Error InvalidName(string name) => Error.Validation("repository.invalid_name", $"'{name}' is not a valid repository name.");

    public static Error InvalidReference(string reference) => Error.Validation("repository.invalid_reference", $"'{reference}' is not a valid commit reference.");

    public static Error NotFound(RepositoryCommit commit) => Error.NotFound("repository.not_found", $"{commit} does not exist or is not accessible.");
}
