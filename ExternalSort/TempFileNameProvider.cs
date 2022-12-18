using System.IO;

namespace ExternalSort;

public class TempFileNameProvider : IFileNameProvider
{
    public string GetUnsortedFileName(int index)
    {
        return Path.GetTempFileName();
    }

    public string GetSortedFileName(int index)
    {
        return Path.GetTempFileName();
    }

    public string GetMergedFileName(int mergeIteration, int[] indices)
    {
        return Path.GetTempFileName();
    }
}
