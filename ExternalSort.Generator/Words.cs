using System;
using System.Linq;
using System.Text;

namespace ExternalSort.Generator;

public class Words
{
    public static readonly string[] All =
        Encoding.UTF8.GetString(LoadResource("words.txt").AsSpan(Encoding.UTF8.Preamble.Length))
            .Split("\n")
            .Select(x => x.Trim())
            .ToArray();

    private static byte[] LoadResource(string name)
    {
        var type = typeof(Words);
        using var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.{name}");
        if (stream == null)
        {
            throw new InvalidOperationException($"Cannot load embedded resource: {name}");
        }

        var length = (int) stream.Length;
        var result = new byte[length];
        var read = 0;
        while (read < length)
        {
            read += stream.Read(result, read, length - read);
        }

        return result;
    }
}
