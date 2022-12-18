namespace ExternalSort;

public interface IFileNameProvider
{
    string GetUnsortedFileName(int index);
    string GetSortedFileName(int index);
    string GetMergedFileName(int mergeIteration, int[] indices);
}
