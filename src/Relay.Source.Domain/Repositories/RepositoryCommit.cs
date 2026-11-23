using System.Text.RegularExpressions;
using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Repositories;

public sealed partial record RepositoryCommit
{
    private RepositoryCommit(string owner, string name, string reference)
    {
        Owner = owner;
        Name = name;
        Reference = reference;
    }

    public string Owner { get; }

    public string Name { get; }

    public string Reference { get; }

    public static Result<RepositoryCommit> Create(string owner, string name, string reference)
    {
        if (!SegmentPattern().IsMatch(owner))
        {
            return RepositoryErrors.InvalidOwner(owner);
        }

        if (!SegmentPattern().IsMatch(name))
        {
            return RepositoryErrors.InvalidName(name);
        }

        return ReferencePattern().IsMatch(reference)
            ? Result.Success(new RepositoryCommit(owner, name, reference))
            : RepositoryErrors.InvalidReference(reference);
    }

    public override string ToString() => $"{Owner}/{Name}@{Reference}";

    [GeneratedRegex("^[A-Za-z0-9_.-]{1,100}$")]
    private static partial Regex SegmentPattern();

    [GeneratedRegex("^[A-Za-z0-9_./-]{1,255}$")]
    private static partial Regex ReferencePattern();
}
