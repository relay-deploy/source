using Relay.Chassis.Primitives;

namespace Relay.Source.Domain.Compose;

public sealed record ComposeRejection(string Service, string File, string Key, Error Error);
