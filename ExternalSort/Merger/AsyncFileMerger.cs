using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExternalSort.FileManager;

namespace ExternalSort.Merger;

/// <summary>
///     note: this code was used to measure performance; not used in real application
/// </summary>
public class AsyncFileMerger : FileMergerBase, IDisposable
{
    private readonly SemaphoreSlim concurrencySemaphore = new(Environment.ProcessorCount);

    public AsyncFileMerger(int filesCount, IFileManager fileManager, IComparer<string?>? comparer = null)
        : base(filesCount, fileManager, comparer)
    {
    }

    protected override Task MergeBatchesAsync(string[] files, int[][] batches, string[] nextFiles)
    {
        var tasks = new Task[batches.Length];
        for (var i = 0; i < batches.Length; i++)
        {
            concurrencySemaphore.Wait();
            var index = i;
            tasks[i] = Task.Run(async () =>
            {
                await MergerHelpers.MergeBatchAsync(files, batches[index], nextFiles[index], Comparer);
                concurrencySemaphore.Release();
            });
        }

        return Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        concurrencySemaphore.Dispose();
    }
}
