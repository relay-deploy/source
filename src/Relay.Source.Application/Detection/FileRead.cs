using Relay.Chassis.Primitives;

namespace Relay.Source.Application.Detection;

public sealed record FileRead(string Path, Result<string> Content);
