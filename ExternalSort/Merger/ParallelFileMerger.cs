using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExternalSort.FileManager;

namespace ExternalSort.Merger;

public class ParallelFileMerger : FileMergerBase
{
    private readonly ParallelOptions options;

    public ParallelFileMerger(int filesCount, IFileManager fileManager, IComparer<string?>? comparer = null)
        : this(filesCount, Environment.ProcessorCount, fileManager, comparer)
    {
    }

    public ParallelFileMerger(
        int filesCount,
        int degreeOfParallelism,
        IFileManager fileManager,
        IComparer<string?>? comparer = null)
        : base(filesCount, fileManager, comparer)
    {
        options = new ParallelOptions {MaxDegreeOfParallelism = degreeOfParallelism};
    }

    protected override Task MergeBatchesAsync(string[] files, int[][] batches, string[] nextFiles)
    {
        Parallel.For(0, batches.Length, options,
            i => MergerHelpers.MergeBatch(files, batches[i], nextFiles[i], Comparer));
        return Task.CompletedTask;
    }
}
