using System.Globalization;
using BenchmarkDotNet.Attributes;
using csFastFloat;

namespace Benchmarks.Suites.Macrobenchmarks;

[MemoryDiagnoser]
public class FloatParsingStrategiesBenchmarks
{
    private const string FloatNumber = "-95.8";

    [Benchmark]
    public float Float_FromString_Simple()
    {
        return float.Parse(FloatNumber, CultureInfo.InvariantCulture.NumberFormat);
    }

    [Benchmark]
    public float Float_FromString_ToDouble_Simple()
    {
        return (float)Convert.ToDouble(FloatNumber);
    }

    [Benchmark]
    public float Float_FromString_LemireFastFloat()
    {
        return FastFloatParser.ParseFloat(FloatNumber);
    }

    [Benchmark]
    public float Int_FromString_Custom()
    {
        // TODO
        return FastFloatParser.ParseFloat(FloatNumber);
    }
}