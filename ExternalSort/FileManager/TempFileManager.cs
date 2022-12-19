using System;
using System.Collections.Concurrent;
using System.IO;

namespace ExternalSort.FileManager;

public class TempFileManager : IFileManager
{
    private readonly ConcurrentDictionary<int, string> initialChunks = new();
    private readonly ConcurrentDictionary<int, ConcurrentBag<string>> mergeIterations = new();

    public string GetUnsortedFileName(int index)
    {
        return initialChunks.GetOrAdd(index, _ => Path.GetTempFileName());
    }

    public void CleanupUnsortedFiles()
    {
    }

    public string GetSortedFileName(int index)
    {
        return initialChunks.GetOrAdd(index, _ => throw new InvalidOperationException("Chunk should already exist"));
    }

    public void CleanupSortedFiles()
    {
        foreach (var chunkFile in initialChunks)
        {
            File.Delete(chunkFile.Value);
        }
    }

    public string GetMergedFileName(int mergeIteration, int[] indices)
    {
        var path = Path.GetTempFileName();
        var bag = mergeIterations.GetOrAdd(mergeIteration, _ => new ConcurrentBag<string>());
        bag.Add(path);
        return path;
    }

    public void CleanupMergedFiles(int mergeIteration)
    {
        if (!mergeIterations.TryRemove(mergeIteration, out var bag))
        {
            return;
        }

        foreach (var path in bag)
        {
            File.Delete(path);
        }
    }
}
