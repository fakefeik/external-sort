using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExternalSort;

public class FileSorter
{
    private readonly IFileNameProvider fileNameProvider;
    private readonly IComparer<string>? comparer;

    public FileSorter(IFileNameProvider fileNameProvider, IComparer<string>? comparer)
    {
        this.fileNameProvider = fileNameProvider;
        this.comparer = comparer;
    }

    public string[] SortFiles(string[] files)
    {
        var sortedFiles = new string[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            sortedFiles[i] = fileNameProvider.GetSortedFileName(i);
        }

        Parallel.For(
            0,
            files.Length,
            new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
            i => SortFile(files[i], sortedFiles[i])
        );

        return sortedFiles;
    }

    private void SortFile(string path, string outPath)
    {
        var lines = File.ReadAllLines(path);
        Array.Sort(lines, comparer);
        File.WriteAllLines(outPath, lines);
    }
}
