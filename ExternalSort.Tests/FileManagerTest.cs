using System;
using System.Threading.Tasks;
using ExternalSort.FileManager;
using ExternalSort.Generator;
using ExternalSort.Merger;
using ExternalSort.Sorter;
using ExternalSort.Splitter;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileManagerTest : TestBase
{
    [TestCase(typeof(TestFileManager))]
    [TestCase(typeof(TempFileManager))]
    [TestCase(typeof(TempFolderFileManager))]
    public async Task Test(Type fileManagerType)
    {
        var fileManager = (IFileManager) Activator.CreateInstance(fileManagerType)!;

        var linesCount = 1024 * 1024 * 8;
        var generator = new CaseGenerator(new CaseGeneratorOptions {LinesCount = linesCount}, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(65536, fileManager);
        var sorter = new ParallelFileSorter(fileManager, FastStringComparer.Instance);
        var merger = new ParallelFileMerger(16, fileManager, FastStringComparer.Instance);

        var externalSort = new ExternalSorter(splitter, sorter, merger, fileManager, new ProgressReporter());
        await externalSort.SortAsync(GetTempFilePath("generated.txt"), GetTempFilePath("output.txt"));

        var actualCount = AssertSorted(GetTempFilePath("output.txt"));
        Assert.That(actualCount, Is.EqualTo(linesCount));
    }
}
