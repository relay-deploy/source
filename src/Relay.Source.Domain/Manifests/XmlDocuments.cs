using System.Xml;
using System.Xml.Linq;

namespace Relay.Source.Domain.Manifests;

internal static class XmlDocuments
{
    private static readonly XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };

    public static XDocument Parse(string content)
    {
        using var reader = XmlReader.Create(new StringReader(content), settings);
        return XDocument.Load(reader);
    }

    public static IEnumerable<XElement> Children(this XElement element, string localName) =>
        element.Elements().Where(child => child.Name.LocalName == localName);

    public static IEnumerable<XElement> DescendantsNamed(this XElement element, string localName) =>
        element.Descendants().Where(child => child.Name.LocalName == localName);
}
