using System;
using System.CommandLine;
using System.IO;
using ExternalSort;

var inputOption = new Option<FileInfo>("--input", "File with content for sort") {IsRequired = true};
var outputOption = new Option<string>("--output", "Output file name") {IsRequired = true};
var chunkFileSizeOption = new Option<int>("--chunk-file-size", () => 24, "File size of temporary files (in megabytes)");
var mergeFilesCountOption = new Option<int>("--merge-files-count", () => 64, "Files count to merge at a time");
var parallelismOption =
    new Option<int>("--parallelism", () => Environment.ProcessorCount,
        "How many cores to use, use all cores by default");

var rootCommand = new RootCommand
{
    inputOption,
    outputOption,
    chunkFileSizeOption,
    mergeFilesCountOption,
    parallelismOption,
};

rootCommand.SetHandler((input, output, chunkFileSizeInMb, mergeFilesCount, parallelism) =>
    {
        var options = new ExternalSorterOptions
        {
            DegreeOfParallelism = parallelism,
            MergeFilesCount = mergeFilesCount,
            SplitFileSize = 1024 * 1024 * chunkFileSizeInMb,
        };
        return ExternalSorter.Build(options).SortAsync(input.FullName, output);
    },
    inputOption,
    outputOption,
    chunkFileSizeOption,
    mergeFilesCountOption,
    parallelismOption
);

await rootCommand.InvokeAsync(args);
