using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExternalSort.Generator;
using ExternalSort.Sorter;
using ExternalSort.Splitter;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileSorterTest : TestBase
{
    [Test]
    public async Task TestSingleFile()
    {
        var sorter = new ParallelFileSorter(new TestFileManager(), FastStringComparer.Instance);
        var files = await sorter.SortFilesAsync(new[] {GetFilePath("example.txt")});
        Assert.That(files, Is.EqualTo(new[] {GetTempFilePath("sorted.0.txt")}));
        AssertSorted(GetTempFilePath("sorted.0.txt"));
    }

    [Test]
    public async Task TestSimple()
    {
        var generator = new CaseGenerator(CaseGeneratorOptions.Default, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 8, new TestFileManager());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        var sorter = new ParallelFileSorter(new TestFileManager(), FastStringComparer.Instance);
        var sorted = await sorter.SortFilesAsync(files);

        foreach (var file in sorted)
        {
            AssertSorted(file);
        }
    }

    [TestCase(typeof(ParallelFileSorter))]
    [TestCase(typeof(AsyncFileSorter))]
    public async Task TestLarge(Type sorterType)
    {
        var generator = new CaseGenerator(new CaseGeneratorOptions
        {
            LinesCount = 1024 * 1024 * 16,
            Seed = 42
        }, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 1024 * 64, new TestFileManager());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        var sorter = (IFileSorter) Activator.CreateInstance(
            sorterType,
            new TestFileManager(),
            FastStringComparer.Instance
        )!;

        var stopwatch = Stopwatch.StartNew();
        var sorted = await sorter.SortFilesAsync(files);
        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        foreach (var file in sorted)
        {
            AssertSorted(file);
        }
    }
}
