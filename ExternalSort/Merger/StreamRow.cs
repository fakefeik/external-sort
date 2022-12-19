using System.IO;

namespace ExternalSort.Merger;

public class StreamRow
{
    public StreamReader Reader { get; set; } = null!;
    public string? CurrentLine { get; set; }
}
