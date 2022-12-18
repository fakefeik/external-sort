using System;
using System.Diagnostics;
using ExternalSort.Generator;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileSorterTest : TestBase
{
    [Test]
    public void TestSingleFile()
    {
        var sorter = new FileSorter(new TestFileNameProvider(), FastStringComparer.Instance);
        var files = sorter.SortFiles(new[] {GetFilePath("example.txt")});
        Assert.That(files, Is.EqualTo(new[] {GetTempFilePath("sorted.0.txt")}));
        AssertSorted(GetTempFilePath("sorted.0.txt"));
    }

    [Test]
    public void TestSimple()
    {
        var generator = new CaseGenerator(CaseGeneratorOptions.Default, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 8, new TestFileNameProvider());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        var sorter = new FileSorter(new TestFileNameProvider(), FastStringComparer.Instance);
        var sorted = sorter.SortFiles(files);

        foreach (var file in sorted)
        {
            AssertSorted(file);
        }
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

        var stopwatch = Stopwatch.StartNew();
        var sorted = sorter.SortFiles(files);
        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        foreach (var file in sorted)
        {
            AssertSorted(file);
        }
    }
}
