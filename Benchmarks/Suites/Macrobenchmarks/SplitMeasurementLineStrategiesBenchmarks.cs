using BenchmarkDotNet.Attributes;
using DataProcessor.Domain;

namespace Benchmarks.Suites.Macrobenchmarks;

[MemoryDiagnoser]
public class SplitMeasurementLineStrategiesBenchmarks
{
    private const string TestString = "TestCityLocation;-12.53";
    private static readonly byte[] TestBytes = "TestCityLocation;-12.53"u8.ToArray();
    private const byte DelimiterByte = 0x3B;

    // Dynamics
    public byte[] StationName;
    public byte[] Measurement;

    /*[Benchmark]
    public string[] Split_String_DefaultStd_ReturnSliceStrings()
    {
        var tokens = TestString.Split(';');
        var first = tokens[0];
        var second = tokens[1];
        return [first, second];
    }

    [Benchmark]
    public SplitTokens Split_String_Span_IndexOf_ReturnSliceStrings()
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
    }*/

    /*[Benchmark]
    public SplitCharsTokens Split_String_Span_Range_Split()
    {
        var span = TestString.AsSpan();
        var delimiterSpan = ";".AsSpan();

        Span<Range> ranges = stackalloc Range[3];
        span.Split(ranges, delimiterSpan); // will always return 2 ranges. add 1 more for good measure.

        return new SplitCharsTokens
        {
            First = span[ranges[0]].ToArray(),
            Second = span[ranges[1]].ToArray()
        };
    }*/

    /*[Benchmark]
    public SplitBytesTokens Split_Bytes_IndexOf_ReturnSliceBytes()
    {
        var delimiterIndex = Array.IndexOf(TestBytes, DelimiterByte);
        return new SplitBytesTokens
        {
            First = TestBytes[..delimiterIndex].ToArray(),
            Second = TestBytes[(delimiterIndex + 1)..].ToArray()
        };
    }*/

    [Benchmark]
    public SplitBytesTokens Split_Bytes_Span_IndexOf_ReturnSliceBytes()
    {
        var span = TestBytes.AsSpan();
        var delimiterIndex = span.IndexOf(DelimiterByte);
        return new SplitBytesTokens
        {
            First = span[..delimiterIndex].ToArray(),
            Second = span[(delimiterIndex + 1)..].ToArray()
        };
    }

    /*[Benchmark]
    public void Split_Bytes_Span_IndexOf_OutVarSliceBytes()
    {
        var span = TestBytes.AsSpan();
        var delimiterIndex = span.IndexOf(DelimiterByte);

        // These would be replaces by out var mutations - however out vars are not allowed in benchmarks by BenchmarkDotNet
        StationName = span[..delimiterIndex].ToArray();
        Measurement = span[(delimiterIndex + 1)..].ToArray();
    }*/

    [Benchmark]
    public UnsafeSplitBytesTokens Split_Bytes_Span_IndexOf_ReturnSliceBytes_Unsafe()
    {
        var span = TestBytes.AsSpan();
        var delimiterIndex = span.IndexOf(DelimiterByte);
        return new UnsafeSplitBytesTokens
        {
            First = span[..delimiterIndex],
            Second = span[(delimiterIndex + 1)..]
        };
    }
}