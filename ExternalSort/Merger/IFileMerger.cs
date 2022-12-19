using System.Threading.Tasks;

namespace ExternalSort.Merger;

public interface IFileMerger
{
    Task MergeAsync(string[] files, string outPath);
}
