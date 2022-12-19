using System.IO;
using ExternalSort.FileManager;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class TestFileManager : IFileManager
{
    public string GetUnsortedFileName(int index)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp", $"unsorted.{index}.txt");
    }

    public void CleanupUnsortedFiles()
    {
    }

    public string GetSortedFileName(int index)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp", $"sorted.{index}.txt");
    }

    public void CleanupSortedFiles()
    {
    }

    public string GetMergedFileName(int mergeIteration, int[] indices)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp",
            $"merged.iteration{mergeIteration}.{indices[0]}-{indices[^1]}.txt");
    }

    public void CleanupMergedFiles(int mergeIteration)
    {
    }
}
