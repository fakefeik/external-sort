using System;
using System.Threading.Tasks;
using ExternalSort.FileManager;
using ExternalSort.Merger;
using ExternalSort.Sorter;
using ExternalSort.Splitter;

namespace ExternalSort;

public class ExternalSorter
{
    private readonly IFileSplitter splitter;
    private readonly IFileSorter sorter;
    private readonly IFileMerger merger;
    private readonly IFileManager fileManager;
    private readonly IProgress<Stage> reporter;

    public static ExternalSorter Build(ExternalSorterOptions options)
    {
        var fileManager = new TempFileManager();
        var splitter = new FileSplitter(options.SplitFileSize, fileManager);
        var sorter = new ParallelFileSorter(options.DegreeOfParallelism, fileManager, FastStringComparer.Instance);
        var merger = new ParallelFileMerger(
            options.MergeFilesCount,
            options.DegreeOfParallelism,
            fileManager,
            FastStringComparer.Instance);

        return new ExternalSorter(splitter, sorter, merger, fileManager, new ProgressReporter());
    }

    public ExternalSorter(
        IFileSplitter splitter,
        IFileSorter sorter,
        IFileMerger merger,
        IFileManager fileManager,
        IProgress<Stage> reporter)
    {
        this.splitter = splitter;
        this.sorter = sorter;
        this.merger = merger;
        this.fileManager = fileManager;
        this.reporter = reporter;
    }

    public async Task SortAsync(string path, string outPath)
    {
        var files = splitter.SplitFile(path);
        reporter.Report(Stage.Split);

        var sorted = await sorter.SortFilesAsync(files);
        reporter.Report(Stage.Sort);

        fileManager.CleanupUnsortedFiles();

        await merger.MergeAsync(sorted, outPath);
        reporter.Report(Stage.Merge);
    }
}
