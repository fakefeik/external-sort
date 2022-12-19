using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExternalSort.Comparers;
using ExternalSort.FileManager;

namespace ExternalSort.Merger;

public abstract class FileMergerBase : IFileMerger
{
    private readonly int filesCount;
    private readonly IFileManager fileManager;

    protected readonly IComparer<StreamRow?> Comparer;

    protected FileMergerBase(int filesCount, IFileManager fileManager, IComparer<string?>? comparer = null)
    {
        if (filesCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(filesCount));
        }

        this.filesCount = filesCount;
        this.fileManager = fileManager;
        Comparer = new RowComparer(comparer ?? Comparer<string?>.Default);
    }

    public async Task MergeAsync(string[] files, string outPath)
    {
        int[][] batches;
        var mergeIteration = 0;
        while ((batches = MergerHelpers.GetBatches(files, filesCount)).Length != 1)
        {
            var nextFiles = new string[batches.Length];
            for (var i = 0; i < batches.Length; i++)
            {
                nextFiles[i] = fileManager.GetMergedFileName(mergeIteration, batches[i]);
            }

            await MergeBatchesAsync(files, batches, nextFiles);

            Cleanup(mergeIteration);

            mergeIteration++;
            files = nextFiles;
        }

        MergerHelpers.MergeBatch(files, batches[0], outPath, Comparer);
        Cleanup(mergeIteration);
    }

    protected abstract Task MergeBatchesAsync(string[] files, int[][] batches, string[] nextFiles);

    private void Cleanup(int mergeIteration)
    {
        if (mergeIteration == 0)
        {
            fileManager.CleanupSortedFiles();
        }
        else
        {
            fileManager.CleanupMergedFiles(mergeIteration - 1);
        }
    }
}
