using System.IO;
using System.Linq;
using ExternalSort.Generator;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class SimpleTest
{
    [Test]
    public void TestSimple()
    {
        var generator = new CaseGenerator(CaseGeneratorOptions.Default, Words.All);
        generator.Generate("generated.txt");

        var sorter = new ExternalSorter(ExternalSorterOptions.Default);
        sorter.Sort("generated.txt", "sorted.txt");

        AssertSorted("sorted.txt");
    }

    private static void AssertSorted(string filename)
    {
        var prevIndex = 0;
        string? prevLine = null;
        foreach (var (index, line) in File.ReadLines(filename).Select(Helpers.ParseLine))
        {
            if (prevLine == null)
            {
                prevIndex = index;
                prevLine = line;
                continue;
            }

            Assert.That(prevLine, Is.LessThanOrEqualTo(line));
            if (prevLine == line)
            {
                Assert.That(prevIndex, Is.LessThanOrEqualTo(index));
            }

            prevIndex = index;
            prevLine = line;
        }
    }
}
