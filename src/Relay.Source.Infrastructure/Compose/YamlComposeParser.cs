using Relay.Chassis.Primitives;
using Relay.Source.Application.Detection;
using Relay.Source.Domain.Compose;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Relay.Source.Infrastructure.Compose;

public sealed class YamlComposeParser : IComposeParser
{
    private static readonly IDeserializer deserializer = new DeserializerBuilder().Build();

    public Result<ComposeDocument> Parse(string path, string content)
    {
        try
        {
            var root = deserializer.Deserialize<object?>(new MergingParser(new Parser(new StringReader(content))));
            var services = root.AsMap().Get("services").AsMap();
            return new ComposeDocument(path, [.. services.Select(service => ComposeServiceMapping.ToService(path, service.Key, service.Value))]);
        }
        catch (YamlException)
        {
            return ComposeErrors.Unreadable(path);
        }
    }
}
