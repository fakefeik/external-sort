using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ExternalSort.Tests;

public abstract class TestBase
{
    [SetUp]
    public void SetUp()
    {
        var tempDirectory = GetTempDirectory();
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, recursive: true);
        }

        Directory.CreateDirectory(tempDirectory);
    }

    protected int AssertSorted(string filename)
    {
        var prevIndex = 0;
        string? prevLine = null;
        var count = 0;
        foreach (var (index, line) in File.ReadLines(filename).Select(ParseLine))
        {
            count++;
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

        return count;
    }


    protected string GetFilePath(string filename)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Files", filename);
    }

    protected string GetTempFilePath(string filename)
    {
        return Path.Combine(GetTempDirectory(), filename);
    }

    protected string GetTempDirectory()
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "Temp");
    }

    private static (int, string) ParseLine(string line)
    {
        var parts = line.Split(". ");
        return (int.Parse(parts[0]), parts[1]);
    }
}
