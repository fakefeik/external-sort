using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExternalSort.Comparers;
using ExternalSort.TournamentTree;
using MoreLinq;

namespace ExternalSort;

public class FileMerger
{
    private readonly int filesCount;
    private readonly IFileNameProvider fileNameProvider;
    private readonly IComparer<string?>? comparer;

    public FileMerger(int filesCount, IFileNameProvider fileNameProvider, IComparer<string?>? comparer)
    {
        if (filesCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(filesCount));
        }

        this.filesCount = filesCount;
        this.fileNameProvider = fileNameProvider;
        this.comparer = comparer;
    }

    public void Merge(string[] files, string outPath)
    {
        int[][] batches;
        var mergeIteration = 0;
        while ((batches = Batch(files)).Length != 1)
        {
            var nextFiles = new string[batches.Length];
            var iteration = mergeIteration;
            for (var i = 0; i < batches.Length; i++)
            {
                MergeInternal(files, batches[i], nextFiles, i, iteration);
            }

            // Parallel.For(
            // 0,
            // batches.Length,
            // new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
            // i => MergeInternal(files, batches[i], nextFiles, i, iteration)
            // );
            mergeIteration++;
            files = nextFiles;
        }

        MergeInternal(files, batches[0], outPath);
    }

    private void MergeInternal(string[] files, int[] indices, string[] nextFiles, int index, int mergeIteration)
    {
        var mergedFileName = fileNameProvider.GetMergedFileName(mergeIteration, indices);
        nextFiles[index] = mergedFileName;
        MergeInternal(files, indices, mergedFileName);
    }

    private void MergeInternal(string[] files, int[] indices, string mergedFileName)
    {
        var streamRows = new StreamRow[indices.Length];
        for (var i = 0; i < indices.Length; i++)
        {
            var reader = new StreamReader(files[indices[i]]);
            streamRows[i] = new StreamRow
            {
                Reader = reader,
                CurrentLine = reader.ReadLine(),
            };
        }

        using var outputWriter = new StreamWriter(mergedFileName);
        try
        {
            var tournamentTree = new TournamentTree<StreamRow>(streamRows, RowComparer.Instance);
            while (tournamentTree.PopRoot() is { } winner)
            {
                outputWriter.WriteLine(winner.CurrentLine);
                tournamentTree.InsertLeaf(winner.Reader.EndOfStream
                    ? null
                    : new StreamRow
                    {
                        Reader = winner.Reader,
                        CurrentLine = winner.Reader.ReadLine(),
                    });
            }
        }
        finally
        {
            foreach (var streamReader in streamRows)
            {
                streamReader.Reader.Dispose();
            }
        }
    }

    private int[][] Batch(string[] files)
    {
        var count = files.Length / filesCount;
        var batchesCount = count + (files.Length % filesCount == 0 ? 0 : 1);
        var batches = Enumerable.Range(0, batchesCount)
            .Select(i => new int[i == count ? files.Length % filesCount : filesCount])
            .ToArray();
        for (var i = 0; i < files.Length; i++)
        {
            batches[i / filesCount][i % filesCount] = i;
        }

        return batches;
    }
}
