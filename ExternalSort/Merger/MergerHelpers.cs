using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExternalSort.TournamentTree;

namespace ExternalSort.Merger;

public static class MergerHelpers
{
    public static async Task MergeBatchAsync(string[] files, int[] indices, string mergedFileName,
        IComparer<StreamRow?> comparer)
    {
        var streamRows = new StreamRow[indices.Length];
        for (var i = 0; i < indices.Length; i++)
        {
            var reader = new StreamReader(files[indices[i]]);
            streamRows[i] = new StreamRow
            {
                Reader = reader,
                CurrentLine = await reader.ReadLineAsync(),
            };
        }

        try
        {
            await using var outputWriter = new StreamWriter(mergedFileName);
            var tournamentTree = new TournamentTree<StreamRow>(streamRows, comparer);
            while (tournamentTree.PopRoot() is { } winner)
            {
                await outputWriter.WriteLineAsync(winner.CurrentLine);
                tournamentTree.InsertLeaf(winner.Reader.EndOfStream
                    ? null
                    : new StreamRow
                    {
                        Reader = winner.Reader,
                        CurrentLine = await winner.Reader.ReadLineAsync(),
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

    public static void MergeBatch(string[] files, int[] indices, string mergedFileName, IComparer<StreamRow?> comparer)
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

        try
        {
            using var outputWriter = new StreamWriter(mergedFileName);
            var tournamentTree = new TournamentTree<StreamRow>(streamRows, comparer);
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

    public static int[][] GetBatches(string[] files, int batchSize)
    {
        var count = files.Length / batchSize;
        var batchesCount = count + (files.Length % batchSize == 0 ? 0 : 1);
        var batches = Enumerable.Range(0, batchesCount)
            .Select(i => new int[i == count ? files.Length % batchSize : batchSize])
            .ToArray();
        for (var i = 0; i < files.Length; i++)
        {
            batches[i / batchSize][i % batchSize] = i;
        }

        return batches;
    }
}
