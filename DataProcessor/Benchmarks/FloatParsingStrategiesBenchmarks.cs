using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace DataProcessor.Benchmarks;

[MemoryDiagnoser]
public class FloatParsingStrategiesBenchmarks
{
    [Benchmark]
    public float Float_FromString_Simple()
    {
        const string randomFloatString = "-95.36";
        return float.Parse(randomFloatString, CultureInfo.InvariantCulture.NumberFormat);
    }
    
    [Benchmark]
    public float Float_FromString_ToDouble_Simple()
    {
        const string randomFloatString = "-95.36";
        return (float)Convert.ToDouble(randomFloatString);
    }
}