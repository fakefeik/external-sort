using System;
using System.Diagnostics;

namespace ExternalSort;

public enum Stage
{
    Split,
    Sort,
    Merge,
}

public class ProgressReporter : IProgress<Stage>
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private long previous;

    public void Report(Stage value)
    {
        var elapsed = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"{value} took {elapsed - previous} ms");
        previous = elapsed;

        if (value == Stage.Merge)
        {
            Console.WriteLine($"Whole process took {elapsed} ms");
        }
    }
}
