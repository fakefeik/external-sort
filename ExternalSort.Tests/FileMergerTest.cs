using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExternalSort.Generator;
using ExternalSort.Merger;
using ExternalSort.Sorter;
using ExternalSort.Splitter;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileMergerTest : TestBase
{
    [TestCase(2)]
    [TestCase(5)]
    [TestCase(10)]
    public async Task SimpleTest(int fileCount)
    {
        var counts = new[] {1, 2, 5, 16, 3, 8, 1, 3, 5, 9, 1, 1};
        var lines = await File.ReadAllLinesAsync(GetFilePath("for-merge.txt"));
        Split(lines, counts);

        var merger = new ParallelFileMerger(fileCount, new TestFileManager(), FastStringComparer.Instance);
        var files = Enumerable.Range(0, 12).Select(i => GetTempFilePath($"for-merge.{i}.txt")).ToArray();
        await merger.MergeAsync(files, GetTempFilePath("merged.txt"));

        var actual = await File.ReadAllLinesAsync(GetTempFilePath("merged.txt"));
        Array.Sort(lines, FastStringComparer.Instance);
        Assert.That(actual, Is.EqualTo(lines));
    }

    /// <summary>
    ///     note: In general we do not need Async in this task because we are not handling thousands of RPS and thus not limited by thread pool resources
    ///     tried to use Async tasks anyway to see what difference does it make
    ///     in AsyncFileSorter we spawn tasks all immediately, in AsyncFileMerger we run Environment.ProcessorCount tasks at a time
    ///     in the end both loose to simple Parallel.For without any async code
    /// </summary>
    /// <param name="mergerType"></param>
    [TestCase(typeof(ParallelFileMerger))]
    [TestCase(typeof(AsyncFileMerger))]
    public async Task TestLarge(Type mergerType)
    {
        var generator = new CaseGenerator(new CaseGeneratorOptions
        {
            LinesCount = 1024 * 1024 * 16,
            Seed = 42
        }, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 1024 * 64, new TestFileManager());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        var sorter = new ParallelFileSorter(new TestFileManager(), FastStringComparer.Instance);
        var sorted = await sorter.SortFilesAsync(files);

        var merger = (IFileMerger) Activator.CreateInstance(
            mergerType,
            8,
            new TestFileManager(),
            FastStringComparer.Instance
        )!;
        var stopwatch = Stopwatch.StartNew();
        await merger.MergeAsync(sorted, GetTempFilePath("sorted.txt"));
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
