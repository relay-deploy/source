using Relay.Chassis.Primitives;
using Relay.Source.Domain.Compose;

namespace Relay.Source.Application.Detection;

public interface IComposeParser
{
    Result<ComposeDocument> Parse(string path, string content);
}
