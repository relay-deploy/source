using System.Text;

namespace Relay.Source.Infrastructure.Compose;

internal static class ComposeText
{
    public static IReadOnlyList<string> SplitOutsideVariables(this string text, char separator)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        var depth = 0;
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (character == '{' && index > 0 && text[index - 1] == '$')
            {
                depth++;
            }
            else if (character == '}' && depth > 0)
            {
                depth--;
            }

            if (character == separator && depth == 0)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(character);
            }
        }

        parts.Add(current.ToString());
        return parts;
    }
}
