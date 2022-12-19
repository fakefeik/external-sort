namespace ExternalSort.FileManager;

public interface IFileManager
{
    string GetUnsortedFileName(int index);
    void CleanupUnsortedFiles();

    string GetSortedFileName(int index);
    void CleanupSortedFiles();

    string GetMergedFileName(int mergeIteration, int[] indices);
    void CleanupMergedFiles(int mergeIteration);
}
