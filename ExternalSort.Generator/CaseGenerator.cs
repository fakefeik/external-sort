using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ExternalSort.Generator;

public class CaseGenerator
{
    private readonly CaseGeneratorOptions options;
    private readonly string[] words;
    private readonly Random random;

    public CaseGenerator(CaseGeneratorOptions options, string[] words)
    {
        this.options = options;
        this.words = words;
        random = options.Seed.HasValue
            ? new Random(options.Seed.Value)
            : new Random();
    }

    public void Generate(string filename)
    {
        using var stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

        var buffer = new byte[options.BufferSize];
        var bufferIndex = 0;
        while (options.LinesCount-- > 0)
        {
            var index = random.Next(options.MinIndex, options.MaxIndex);
            var wordsCount = random.Next(options.MinWords, options.MaxWords);
            var ws = Enumerable.Range(0, wordsCount).Select(_ => words[random.Next(0, words.Length)]).ToArray();
            var s = Encoding.UTF8.GetBytes($"{index}. {string.Join(" ", ws)}\n");
            if (bufferIndex + s.Length < buffer.Length)
            {
                Array.Copy(s, 0, buffer, bufferIndex, s.Length);
                bufferIndex += s.Length;
            }
            else
            {
                stream.Write(buffer, 0, bufferIndex);
                bufferIndex = s.Length;
                Array.Copy(s, 0, buffer, 0, s.Length);
            }
        }

        stream.Write(buffer, 0, bufferIndex - 1);
    }
}
