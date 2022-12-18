using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExternalSort.Generator;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileMergerTest : TestBase
{
    [TestCase(2)]
    [TestCase(5)]
    [TestCase(10)]
    public void SimpleTest(int fileCount)
    {
        var counts = new[] {1, 2, 5, 16, 3, 8, 1, 3, 5, 9, 1, 1};
        var lines = File.ReadAllLines(GetFilePath("for-merge.txt"));
        Split(lines, counts);

        var merger = new FileMerger(fileCount, new TestFileNameProvider(), FastStringComparer.Instance);
        var files = Enumerable.Range(0, 12).Select(i => GetTempFilePath($"for-merge.{i}.txt")).ToArray();
        merger.Merge(files, GetTempFilePath("merged.txt"));

        var actual = File.ReadAllLines(GetTempFilePath("merged.txt"));
        Array.Sort(lines, FastStringComparer.Instance);
        Assert.That(actual, Is.EqualTo(lines));
    }

    [Test]
    public void TestLarge()
    {
        var generator = new CaseGenerator(new CaseGeneratorOptions
        {
            LinesCount = 1024 * 1024 * 16,
            Seed = 42
        }, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 1024 * 64, new TestFileNameProvider());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        var sorter = new FileSorter(new TestFileNameProvider(), FastStringComparer.Instance);
        var sorted = sorter.SortFiles(files);

        var merger = new FileMerger(8, new TestFileNameProvider(), FastStringComparer.Instance);
        var stopwatch = Stopwatch.StartNew();
        merger.Merge(sorted, GetTempFilePath("sorted.txt"));
        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        AssertSorted(GetTempFilePath("sorted.txt"));
    }

    private void Split(string[] lines, int[] counts)
    {
        var read = 0;
        var index = 0;
        foreach (var count in counts)
        {
            var currentLines = lines.Skip(read).Take(count).ToArray();
            Array.Sort(currentLines, FastStringComparer.Instance);
            File.WriteAllLines(GetTempFilePath($"for-merge.{index++}.txt"), currentLines);
            read += count;
        }
    }
}
