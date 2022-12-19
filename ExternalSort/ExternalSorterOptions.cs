using System;

namespace ExternalSort;

public class ExternalSorterOptions
{
    public static readonly ExternalSorterOptions Default = new()
    {
        DegreeOfParallelism = Environment.ProcessorCount,
        MergeFilesCount = 64,
        SplitFileSize = 1024 * 1024 * 24, // 24MB Files
    };

    public int SplitFileSize { get; set; }
    public int MergeFilesCount { get; set; }
    public int DegreeOfParallelism { get; set; }
}
