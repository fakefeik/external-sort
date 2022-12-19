using System.Collections.Generic;
using ExternalSort.Merger;

namespace ExternalSort.Comparers;

public class RowComparer : IComparer<StreamRow?>
{
    private readonly IComparer<string?> comparer;

    public RowComparer(IComparer<string?> comparer)
    {
        this.comparer = comparer;
    }

    public int Compare(StreamRow? x, StreamRow? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y?.CurrentLine)) return -1;
        if (ReferenceEquals(null, x?.CurrentLine)) return 1;
        return comparer.Compare(x.CurrentLine, y.CurrentLine);
    }
}
