namespace ExternalSort.Generator;

public class CaseGeneratorOptions
{
    public static CaseGeneratorOptions Default = new();

    public int? Seed { get; set; }
    public int LinesCount { get; set; } = 4096;
    public int BufferSize { get; set; } = 1024;
    public int MinIndex { get; set; } = 0;
    public int MaxIndex { get; set; } = 32768;
    public int MinWords { get; set; } = 2;
    public int MaxWords { get; set; } = 16;
}
