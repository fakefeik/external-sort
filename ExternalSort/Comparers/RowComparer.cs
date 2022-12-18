using System.Collections.Generic;

namespace ExternalSort.Comparers;

public class RowComparer : IComparer<StreamRow?>
{
    public static readonly RowComparer Instance = new();

    public int Compare(StreamRow? x, StreamRow? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y?.CurrentLine)) return -1;
        if (ReferenceEquals(null, x?.CurrentLine)) return 1;
        return FastStringComparer.Instance.Compare(x.CurrentLine, y.CurrentLine);
    }
}
