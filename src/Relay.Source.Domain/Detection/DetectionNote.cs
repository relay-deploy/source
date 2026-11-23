using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Detection;

public sealed record DetectionNote(string Code, string Subject, string Message)
{
    public static DetectionNote From(Error error, string subject) => new(error.Code, subject, error.Message);
}
