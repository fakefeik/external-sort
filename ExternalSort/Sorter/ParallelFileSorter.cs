using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExternalSort.FileManager;

namespace ExternalSort.Sorter;

public class ParallelFileSorter : IFileSorter
{
    private readonly IFileManager fileManager;
    private readonly IComparer<string>? comparer;
    private readonly ParallelOptions parallelOptions;


    public ParallelFileSorter(int degreeOfParallelism, IFileManager fileManager, IComparer<string>? comparer)
    {
        this.fileManager = fileManager;
        this.comparer = comparer;
        parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = degreeOfParallelism};
    }

    public ParallelFileSorter(IFileManager fileManager, IComparer<string>? comparer)
        : this(Environment.ProcessorCount, fileManager, comparer)
    {
    }

    public Task<string[]> SortFilesAsync(string[] files)
    {
        var sortedFiles = new string[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            sortedFiles[i] = fileManager.GetSortedFileName(i);
        }

        Parallel.For(0, files.Length, parallelOptions, i => SortFile(files[i], sortedFiles[i]));

        return Task.FromResult(sortedFiles);
    }

    private void SortFile(string path, string outPath)
    {
        var lines = File.ReadAllLines(path);
        Array.Sort(lines, comparer);
        File.WriteAllLines(outPath, lines);
    }
}
