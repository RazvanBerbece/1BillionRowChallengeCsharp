using BenchmarkDotNet.Attributes;
using DataProcessor.Domain;

namespace DataProcessor.Benchmarks;

[MemoryDiagnoser(false)]
public class MapOperationsStrategiesBenchmarks
{
    private static readonly (string, string) TestMapTypeStrings = ("WeatherStationName", "-25.5");
    private static readonly (byte[], byte[]) TestMapTypeBytes = ("WeatherStationName"u8.ToArray(), "-25.5"u8.ToArray());
    private static readonly (string, MeasurementData) TestMapTypeStringStruct = ("WeatherStationName", new MeasurementData());
    private static readonly (byte[], MeasurementData) TestMapTypeBytesStruct = ("WeatherStationName"u8.ToArray(), new MeasurementData());
    
    // TODO
}