using BenchmarkDotNet.Attributes;
using FileManager.Domain;

namespace FileManager.Benchmarks;

[MemoryDiagnoser(false)]
public class SplitMeasurementLineStrategiesBenchmarks
{

    private static readonly string TestString = "TestCityLocation;-12.53";
    private static readonly byte[] TestBytes = "TestCityLocation;-12.53"u8.ToArray();
    private static readonly byte DelimiterByte = 0x3B;
    
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
    
    [Benchmark]
    public SplitBytesTokens Split_Bytes_IndexOfSlice()
    {
        var delimiterIndex = Array.IndexOf(TestBytes, DelimiterByte);
        return new SplitBytesTokens
        {
            First = TestBytes[..delimiterIndex].ToArray(),
            Second = TestBytes[(delimiterIndex + 1)..].ToArray()
        };
    }
    
    [Benchmark]
    public SplitBytesTokens Split_Bytes_Span_IndexOfSlice()
    {
        var span = TestBytes.AsSpan();
        var delimiterIndex = span.IndexOf(DelimiterByte);
        return new SplitBytesTokens
        {
            First = span[..delimiterIndex].ToArray(),
            Second = span[(delimiterIndex + 1)..].ToArray()
        };
    }
}