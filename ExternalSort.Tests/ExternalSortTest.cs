using System.Threading.Tasks;
using ExternalSort.Generator;
using NUnit.Framework;

namespace ExternalSort.Tests;

public class ExternalSortTest : TestBase
{
    [Test]
    public async Task TestTwoWordsWord()
    {
        var linesCount = 1024 * 1024 * 8;
        var generatorOptions = new CaseGeneratorOptions
        {
            LinesCount = linesCount,
            Seed = 42,
            MinWords = 1,
            MaxWords = 3,
            MinIndex = 1000,
            MaxIndex = 10000,
        };
        var generator = new CaseGenerator(generatorOptions, new[] {"first", "second"});
        generator.Generate(GetTempFilePath("generated.txt"));

        await ExternalSorter
            .Build(ExternalSorterOptions.Default)
            .SortAsync(GetTempFilePath("generated.txt"), GetTempFilePath("output.txt"));
        var actualLines = AssertSorted(GetTempFilePath("output.txt"));
        Assert.That(actualLines, Is.EqualTo(linesCount));
    }

    [TestCase(1024 * 1024 * 16)] // approx 1GB
    [TestCase(1024 * 1024 * 16 * 10)] // 10GB
    [TestCase(1024 * 1024 * 16 * 100, Explicit = true)] // 100GB
    public async Task StressTest(int lines)
    {
        // more predictable options
        var generatorOptions = new CaseGeneratorOptions
        {
            LinesCount = lines,
            Seed = 42,
            MinWords = 10,
            MaxWords = 12,
            MinIndex = 1000,
            MaxIndex = 10000,
        };
        var generator = new CaseGenerator(generatorOptions, Words.All);
        generator.Generate(GetTempFilePath("generated.txt"));

        await ExternalSorter
            .Build(ExternalSorterOptions.Default)
            .SortAsync(GetTempFilePath("generated.txt"), GetTempFilePath("output.txt"));
        var actualLines = AssertSorted(GetTempFilePath("output.txt"));
        Assert.That(actualLines, Is.EqualTo(lines));
    }
}
