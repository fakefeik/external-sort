using System.CommandLine;
using System.CommandLine.Binding;

namespace ExternalSort.Generator;

public class OptionsBinder : BinderBase<CaseGeneratorOptions>
{
    private readonly Option<int?> seedOption;
    private readonly Option<int> linesOption;
    private readonly Option<int> bufferSizeOption;
    private readonly Option<int> minIndexOption;
    private readonly Option<int> maxIndexOption;
    private readonly Option<int> minWordsOption;
    private readonly Option<int> maxWordsOption;

    public OptionsBinder(
        Option<int?> seedOption,
        Option<int> linesOption,
        Option<int> bufferSizeOption,
        Option<int> minIndexOption,
        Option<int> maxIndexOption,
        Option<int> minWordsOption,
        Option<int> maxWordsOption)
    {
        this.seedOption = seedOption;
        this.linesOption = linesOption;
        this.bufferSizeOption = bufferSizeOption;
        this.minIndexOption = minIndexOption;
        this.maxIndexOption = maxIndexOption;
        this.minWordsOption = minWordsOption;
        this.maxWordsOption = maxWordsOption;
    }

    protected override CaseGeneratorOptions GetBoundValue(BindingContext bindingContext) =>
        new()
        {
            Seed = bindingContext.ParseResult.GetValueForOption(seedOption),
            LinesCount = bindingContext.ParseResult.GetValueForOption(linesOption),
            BufferSize = bindingContext.ParseResult.GetValueForOption(bufferSizeOption),
            MinIndex = bindingContext.ParseResult.GetValueForOption(minIndexOption),
            MaxIndex = bindingContext.ParseResult.GetValueForOption(maxIndexOption),
            MinWords = bindingContext.ParseResult.GetValueForOption(minWordsOption),
            MaxWords = bindingContext.ParseResult.GetValueForOption(maxWordsOption),
        };
}
