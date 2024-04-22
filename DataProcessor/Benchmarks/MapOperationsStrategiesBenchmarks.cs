using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using DataProcessor.Domain;
using DataProcessor.Extensions;

namespace DataProcessor.Benchmarks;

[MemoryDiagnoser]
public class MapOperationsStrategiesBenchmarks
{
    private const int MapCapacity = 65792; // anything higher seems to lead to higher mean time and allocated memory

    /*
    [Benchmark]
    public void Map_Simple_AddOrEdit_NoCapacity_StringNames()
    {
        var measurementsMap = new Dictionary<string, MeasurementData>();
        
        const string stationName = "StationName_";
        
        // Step 1 - Add new entries
        for (var i = 0; i < 5000; ++i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            if (!measurementsMap.TryGetValue(randomName, out var measurement))
            {
                measurementsMap.Add(randomName, new MeasurementData
                {
                    Count = 1,
                    Min = randomMeasurement,
                    Max = randomMeasurement,
                    Sum = randomMeasurement
                });
            }
            else
            {
                measurement.Count =+ 1;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement); 
            }
        }
        
        // Step 2 - Add new AND update existing entries
        for (var i = 10000; i >= 0; --i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            if (!measurementsMap.TryGetValue(randomName, out var measurement))
            {
                measurementsMap.Add(randomName, new MeasurementData
                {
                    Count = 1,
                    Min = randomMeasurement,
                    Max = randomMeasurement,
                    Sum = randomMeasurement
                });
            }
            else
            {
                measurement.Count =+ 1;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement); 
            }
        }
    }
    */
    
    /*[Benchmark]
    public void Map_Simple_AddOrEdit_WithCapacity_StringNames()
    {
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);
        
        const string stationName = "StationName_";
        
        // Step 1 - Add new entries
        for (var i = 0; i < 5000; ++i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            if (!measurementsMap.TryGetValue(randomName, out var measurement))
            {
                measurementsMap.Add(randomName, new MeasurementData
                {
                    Count = 1,
                    Min = randomMeasurement,
                    Max = randomMeasurement,
                    Sum = randomMeasurement
                });
            }
            else
            {
                measurement.Count =+ 1;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement); 
            }
        }
        
        // Step 2 - Add new AND update existing entries
        for (var i = 10000; i >= 0; --i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            if (!measurementsMap.TryGetValue(randomName, out var measurement))
            {
                measurementsMap.Add(randomName, new MeasurementData
                {
                    Count = 1,
                    Min = randomMeasurement,
                    Max = randomMeasurement,
                    Sum = randomMeasurement
                });
            }
            else
            {
                measurement.Count =+ 1;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement); 
            }
        }
    }*/
    
    [Benchmark]
    public void Map_Simple_AddOrEdit_WithCapacity_RefMarshal_StringNames()
    {
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);
        
        const string stationName = "StationName_";
        
        // Step 1 - Add new entries
        for (var i = 0; i < 5000; ++i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            
            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, randomName, out var exists);
            if (exists)
            {
                // The entry existed already, so update the existing ref
                measurement.Count++;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement);
            }
            else
            {
                // A new entry was created and the reference returned
                measurement.Count = 1;
                measurement.Sum = randomMeasurement;
                measurement.Max = randomMeasurement;
                measurement.Min = randomMeasurement;
            }
        }
        
        // Step 2 - Add new AND update existing entries
        for (var i = 10000; i >= 0; --i)
        {
            var randomMeasurement = Random.Shared.NextFloatRange((float)-99.9, (float)99.9);
            var randomName = $"{stationName}{i}";
            
            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, randomName, out var exists);
            if (exists)
            {
                // The entry existed already, so update the existing ref
                measurement.Count++;
                measurement.Sum += randomMeasurement;
                measurement.Max = Math.Max(measurement.Max, randomMeasurement);
                measurement.Min = Math.Min(measurement.Min, randomMeasurement);
            }
            else
            {
                // A new entry was created and the reference returned
                measurement.Count = 1;
                measurement.Sum = randomMeasurement;
                measurement.Max = randomMeasurement;
                measurement.Min = randomMeasurement;
            }
        }
    }
    
}