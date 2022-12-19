using System;
using System.IO;

namespace ExternalSort.FileManager;

public class TempFolderFileManager : IFileManager
{
    private readonly string tempDirectory;
    private int lastIteration = -1;

    public TempFolderFileManager()
    {
        tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }

        Directory.CreateDirectory(tempDirectory);
    }

    public string GetUnsortedFileName(int index)
    {
        return Path.Combine(tempDirectory, $"{index}.chunk");
    }

    public void CleanupUnsortedFiles()
    {
    }

    public string GetSortedFileName(int index)
    {
        return Path.Combine(tempDirectory, $"{index}.chunk");
    }

    public void CleanupSortedFiles()
    {
    }

    public string GetMergedFileName(int mergeIteration, int[] indices)
    {
        lastIteration = mergeIteration;
        return Path.Combine(tempDirectory, $"{mergeIteration}.{indices[0]}-{indices[^1]}.chunk");
    }

    public void CleanupMergedFiles(int mergeIteration)
    {
        if (mergeIteration == lastIteration)
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }
}
