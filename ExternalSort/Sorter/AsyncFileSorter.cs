using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExternalSort.FileManager;

namespace ExternalSort.Sorter;

/// <summary>
///     note: this code was used to measure performance; not used in real application
/// </summary>
public class AsyncFileSorter : IFileSorter
{
    private readonly IFileManager fileManager;
    private readonly IComparer<string>? comparer;

    public AsyncFileSorter(IFileManager fileManager, IComparer<string>? comparer)
    {
        this.fileManager = fileManager;
        this.comparer = comparer;
    }

    public async Task<string[]> SortFilesAsync(string[] files)
    {
        var sortedFiles = new string[files.Length];
        var tasks = new Task[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            sortedFiles[i] = fileManager.GetSortedFileName(i);
            tasks[i] = SortFile(files[i], sortedFiles[i]);
        }

        await Task.WhenAll(tasks);
        return sortedFiles;
    }

    private async Task SortFile(string path, string outPath)
    {
        var lines = await File.ReadAllLinesAsync(path);
        Array.Sort(lines, comparer);
        await File.WriteAllLinesAsync(outPath, lines);
    }
}
