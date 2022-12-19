using System.Threading.Tasks;

namespace ExternalSort.Sorter;

public interface IFileSorter
{
    Task<string[]> SortFilesAsync(string[] files);
}
