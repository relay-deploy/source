using System.Diagnostics.CodeAnalysis;

namespace Relay.Source.Domain.Modules;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "BuildStack is the ruled domain term.")]
public enum BuildStack
{
    Node,
    Static,
    Java,
    Go,
    Python,
    Dotnet,
    Rust,
    Custom,
}
