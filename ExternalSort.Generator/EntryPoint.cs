using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ExternalSort.Generator;

var seedOption = new Option<int?>("--seed", () => null, "Random seed");

var linesOption =
    new Option<int>("--lines-count", "Number of lines to be produced by generator") {IsRequired = true};

var bufferSizeOption = new Option<int>(
    "--buffer-size",
    () => 1024 * 8,
    "An option whose argument is parsed as an int.");

var minIndexOption = new Option<int>("--min-index", () => 0, "Min index of first number in line");
var maxIndexOption = new Option<int>("--max-index", () => 32768, "Max index of first number in line");
var minWordsOption = new Option<int>("--min-words", () => 2, "Max index of first number in line");
var maxWordsOption = new Option<int>("--max-words", () => 16, "Max index of first number in line");
var wordsOption = new Option<FileInfo?>(
    "--words",
    () => null,
    "The file with words to be used in generation separated by space or endline");
var outputOption = new Option<string>("--output", () => "gen.txt", "Output file name");

var rootCommand = new RootCommand
{
    wordsOption,
    outputOption,
    seedOption,
    linesOption,
    bufferSizeOption,
    minIndexOption,
    maxIndexOption,
    minWordsOption,
    maxWordsOption
};

rootCommand.SetHandler((options, words, output) =>
    {
        var wordsToUse = words == null
            ? Words.All
            : File.ReadAllText(words.FullName)
                .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var generator = new CaseGenerator(options, wordsToUse);

        var stopwatch = Stopwatch.StartNew();

        generator.Generate(output);

        stopwatch.Stop();
        Console.WriteLine($"Output written to {output}");
        Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms");
        return Task.CompletedTask;
    },
    new OptionsBinder(seedOption,
        linesOption,
        bufferSizeOption,
        minIndexOption,
        maxIndexOption,
        minWordsOption,
        maxWordsOption),
    wordsOption,
    outputOption
);

await rootCommand.InvokeAsync(args);
