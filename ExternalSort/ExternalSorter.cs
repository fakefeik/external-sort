using System;
using System.IO;

namespace ExternalSort;

public class ExternalSorter
{
    private readonly ExternalSorterOptions options;

    public ExternalSorter(ExternalSorterOptions options)
    {
        this.options = options;
    }

    public void Sort(string path, string outPath)
    {
        var lines = File.ReadAllLines(path);
        Array.Sort(lines, (string1, string2) =>
        {
            var (index1, line1) = Helpers.ParseLine(string1);
            var (index2, line2) = Helpers.ParseLine(string2);
            if (line1 == line2)
            {
                return index1.CompareTo(index2);
            }

            return string.Compare(line1, line2, StringComparison.OrdinalIgnoreCase);
        });
        File.WriteAllLines(outPath, lines);
    }
}
