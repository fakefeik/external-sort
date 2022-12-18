using System.IO;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class TestFileNameProvider : IFileNameProvider
{
    public string GetUnsortedFileName(int index)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp", $"unsorted.{index}.txt");
    }

    public string GetSortedFileName(int index)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp", $"sorted.{index}.txt");
    }

    public string GetMergedFileName(int mergeIteration, int[] indices)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp",
            $"merged.iteration{mergeIteration}.{indices[0]}-{indices[^1]}.txt");
    }
}
