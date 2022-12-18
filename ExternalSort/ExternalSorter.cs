using System;
using System.IO;

namespace ExternalSort;

public class ExternalSorter
{
    private readonly FileSplitter splitter;
    private readonly FileSorter sorter;
    private readonly FileMerger merger;

    public ExternalSorter(FileSplitter splitter, FileSorter sorter, FileMerger merger)
    {
        this.splitter = splitter;
        this.sorter = sorter;
        this.merger = merger;
    }

    public void Sort(string path, string outPath)
    {
        var files = splitter.SplitFile(path);
        var sorted = sorter.SortFiles(files);
        merger.Merge(sorted, outPath);
    }
}
