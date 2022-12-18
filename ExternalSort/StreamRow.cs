using System.IO;

namespace ExternalSort;

public class StreamRow
{
    public StreamReader Reader { get; set; }
    public string? CurrentLine { get; set; }
}
