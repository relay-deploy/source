using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Compose;

public sealed record ComposeViolation(string Key, Error Error);
