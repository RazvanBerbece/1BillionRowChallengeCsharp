using BenchmarkDotNet.Attributes;
using FileManager.Domain;

namespace FileManager.Benchmarks;

[MemoryDiagnoser(false)]
public class SplitStrategiesBenchmarks
{

    private static readonly string TestString = "TestCityLocation;-12.53";
    
    [Benchmark]
    public string[] Split_String_DefaultStd()
    {
        var tokens = TestString.Split(';');
        var first = tokens[0];
        var second = tokens[1];
        return [first, second];
    }
    
    [Benchmark]
    public SplitTokens Split_String_Span_IndexOfCharSlice()
    {
        var span = TestString.AsSpan();
        var delimiter = ";".AsSpan();
        
        // Retrieve tokens
        var delimiterIndex = span.IndexOf(delimiter, StringComparison.Ordinal);
        var first = span[..delimiterIndex];
        var second = span[(delimiterIndex + 1)..];

        return new SplitTokens
        {
            First = first.ToString(),
            Second = second.ToString()
        };
    }
}