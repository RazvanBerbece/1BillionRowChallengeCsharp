using BenchmarkDotNet.Attributes;
using DataProcessor.Domain;

namespace Benchmarks.Suites.Macrobenchmarks;

[MemoryDiagnoser]
public class MapHashingOperationsStrategiesBenchmarks
{
    private static readonly string TestName = "TestCityLocation";
    private static readonly string TestMeasurement = "-12.53";

    private static readonly byte[] TestBytesName = "TestCityLocation"u8.ToArray();
    private static readonly byte[] TestBytesMeasurement = "-12.53"u8.ToArray();

    private const int MapCapacity = 65792; // use optimal value

    private Dictionary<string, MeasurementData> DefaultComparerMeasurementData;
    // private Dictionary<string, MeasurementData> CustomComparerMeasurementData;

    [GlobalSetup]
    public void Setup()
    {
        DefaultComparerMeasurementData = new Dictionary<string, MeasurementData>(MapCapacity)
        {
            {
                TestName, new MeasurementData
                {
                    Sum = 0,
                    Min = 0,
                    Max = 0,
                    Count = 0
                }
            }
        };
        /*CustomComparerMeasurementData = new Dictionary<string, MeasurementData>(
            MapCapacity,
            new MeasurementsDictionaryComparer())
        {
            {
                TestName, new MeasurementData
                {
                    Sum = 0,
                    Min = 0,
                    Max = 0,
                    Count = 0
                }
            }
        };*/
    }

    [Benchmark]
    public int Map_Default_GetHashCode()
    {
        return DefaultComparerMeasurementData[TestName].GetHashCode();
    }

    /*[Benchmark]
    public int Map_Custom_GetHashCode()
    {
        return CustomComparerMeasurementData[TestName].GetHashCode();
    }*/
}