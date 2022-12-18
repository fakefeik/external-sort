using System;
using System.Collections.Generic;

namespace ExternalSort;

public class FastStringComparer : IComparer<string?>
{
    public static readonly FastStringComparer Instance = new();

    public int Compare(string? x, string? y)
    {
        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        var dot1 = x.IndexOf(". ", StringComparison.OrdinalIgnoreCase);
        var dot2 = y.IndexOf(". ", StringComparison.OrdinalIgnoreCase);
        var comparison = string.Compare(x, dot1 + 2, y, dot2 + 2, Math.Max(x.Length, y.Length),
            StringComparison.OrdinalIgnoreCase);

        if (comparison != 0)
        {
            return comparison;
        }

        return int.Parse(x.AsSpan(0, dot1)).CompareTo(int.Parse(y.AsSpan(0, dot2)));
    }
}
