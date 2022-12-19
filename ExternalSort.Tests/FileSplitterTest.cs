using System.IO;
using System.Linq;
using ExternalSort.Generator;
using ExternalSort.Splitter;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class FileSplitterTest : TestBase
{
    [Test]
    public void Should_ProduceSingleFile_ForSmallFile()
    {
        var splitter = new FileSplitter(1024, new TestFileManager());
        splitter.SplitFile(GetFilePath("example.txt"));

        Assert.That(Directory.EnumerateFiles(GetTempDirectory()), Has.One.Items);
        var expectedContent = File.ReadAllText(GetFilePath("example.txt"));
        var actualContent = File.ReadAllText(GetTempFilePath("unsorted.0.txt"));
        Assert.That(expectedContent, Is.EqualTo(actualContent));
    }

    [Test]
    public void Should_ProduceSingleFile_ForLargeFileWithSingleLine()
    {
        var splitter = new FileSplitter(512, new TestFileManager());
        var files = splitter.SplitFile(GetFilePath("split-single-large.txt"));

        Assert.That(Directory.EnumerateFiles(GetTempDirectory()), Has.One.Items);
        Assert.That(files, Is.EqualTo(new[] {GetTempFilePath("unsorted.0.txt")}));
        var expectedContent = File.ReadAllText(GetFilePath("split-single-large.txt"));
        var actualContent = File.ReadAllText(GetTempFilePath("unsorted.0.txt"));
        Assert.That(expectedContent, Is.EqualTo(actualContent));
    }

    [Test]
    public void Should_SplitInTwoFiles()
    {
        var splitter = new FileSplitter(50, new TestFileManager());
        var files = splitter.SplitFile(GetFilePath("example.txt"));
        Assert.That(files, Is.EquivalentTo(Enumerable.Range(0, 2).Select(i => GetTempFilePath($"unsorted.{i}.txt"))));
        Assert.That(Directory.EnumerateFiles(GetTempDirectory()), Has.Exactly(2).Items);

        var expected = File.ReadAllLines(GetFilePath("example.txt"));
        var actual1 = File.ReadAllLines(GetTempFilePath("unsorted.0.txt"));
        var actual2 = File.ReadAllLines(GetTempFilePath("unsorted.1.txt"));
        Assert.That(actual1, Is.EquivalentTo(expected.Take(2)));
        Assert.That(actual2, Is.EquivalentTo(expected.Skip(2)));
    }

    [Test]
    public void Should_SplitInFiveFiles()
    {
        var splitter = new FileSplitter(1, new TestFileManager());
        var files = splitter.SplitFile(GetFilePath("example.txt"));
        Assert.That(files, Is.EquivalentTo(Enumerable.Range(0, 5).Select(i => GetTempFilePath($"unsorted.{i}.txt"))));
        Assert.That(Directory.EnumerateFiles(GetTempDirectory()), Has.Exactly(5).Items);
        foreach (var (index, line) in File.ReadAllLines(GetFilePath("example.txt")).Select((x, i) => (i, x)))
        {
            var actual = File.ReadAllText(GetTempFilePath($"unsorted.{index}.txt"));
            Assert.That(actual, Is.EqualTo(line));
        }
    }

    [Test]
    public void Should_SplitLargeFile()
    {
        var generator = new CaseGenerator(new CaseGeneratorOptions {LinesCount = 1024 * 1024 * 8}, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        var splitter = new FileSplitter(1024 * 1024 * 32, new TestFileManager());
        var files = splitter.SplitFile(GetTempFilePath("generated.txt"));

        Assert.That(Directory.EnumerateFiles(GetTempDirectory()), Has.Exactly(files.Length + 1).Items);

        using var largeFile = new StreamReader(GetTempFilePath("generated.txt"));
        var currentFile = 0;
        var currentLines = File.ReadAllLines(files[currentFile]);
        var currentLineIndex = 0;
        while (largeFile.ReadLine() is { } currentLine)
        {
            if (currentLineIndex >= currentLines.Length)
            {
                currentLineIndex = 0;
                currentLines = File.ReadAllLines(files[++currentFile]);
            }

            Assert.That(currentLines[currentLineIndex++], Is.EqualTo(currentLine));
        }
    }
}
