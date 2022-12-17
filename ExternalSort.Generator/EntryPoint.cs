using System;
using System.Diagnostics;
using ExternalSort.Generator;

var options = new CaseGeneratorOptions
{
    Seed = 10,
    LinesCount = 1024 * 1024 * 16,
    BufferSize = 1024 * 8,
    MinIndex = 12,
    MaxIndex = 16,
    MinWords = 8,
    MaxWords = 9,
};

var generator = new CaseGenerator(options, Words.All);

var stopwatch = Stopwatch.StartNew();

generator.Generate("gen.txt");

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);
