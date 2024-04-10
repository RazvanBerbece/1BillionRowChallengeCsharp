using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using FileManager.Domain;
using FileManager.Interfaces;
using FileManager.Utils;

namespace FileManager;

/// <summary>
/// Custom implementation of a file manager that provides methods to read data from a file and to write to one using
/// more efficient streams.
/// </summary>
public class StreamFileManager: IFileManager
{
    public StreamFileManager()
    {
    }

    public Dictionary<ByteSpan, MeasurementData> ReadBytesFromFileWithCustomSpanKeys(string filepath)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, MeasurementData> ReadTextFromFileInCustomStruct(string filepath)
    {
        var measurementsMap = new Dictionary<string, MeasurementData>(10000); // 10k unique station names, as per the spec
        
        const int bufferSize = 10 * 1024 * 1024;
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: bufferSize,
            detectEncodingFromByteOrderMarks: false);

        var debug = false;
        const int totalCount = 1000000000; // this won't change
        const int countResetThreshold = 10000000;
        var iterations = totalCount / countResetThreshold;
        
        var auxIndex = 0;
        var count = 0;
        
        // Declare these outside to minimise amount of allocations inside the read loop
        string? measurementLine;
        string[] measurementTokens;
        SplitTokens measurementTokensCustom;
        string stationName;
        string stationMeasurement;
        char[][] measurementTokensChar2d;
        var millionEntryWatch = Stopwatch.StartNew();
        while (!reader.EndOfStream)
        {
            // POTENTIAL SLOWDOWN - String constructor alloc
            measurementLine = reader.ReadLine();
            
            // Retrieve tokens - Station name and measurement value (CUSTOM FOR LOOP SPLIT)
            /*measurementTokensChar2d = StringExtensions.SimpleSpanLoopBuilderSplit(measurementLine, ';');
            stationName = new string(measurementTokensChar2d[0]);
            stationMeasurement = new string(measurementTokensChar2d[1]);*/

            // Retrieve tokens - Station name and measurement value (CUSTOM SPAN SPLIT)
            /*measurementTokensCustom = StringExtensions.SimpleSpanIndexSplit(measurementLine, delimiterSpan);
            stationName = measurementTokens.First;
            stationMeasurement = measurementTokens.Second;*/
            
            // Retrieve tokens - Station name and measurement value (STANDARD SPLIT)
            // POTENTIAL SLOWDOWN - String parsing / splitting
            measurementTokens = measurementLine.Split(';');
            stationName = measurementTokens[0];
            stationMeasurement = measurementTokens[1];
            
            // POTENTIAL SLOWDOWN - String to Float parsing
            var parsedMeasurementValue = float.Parse(stationMeasurement);
            
            // Update result map - Add new or update existing measurements
            // POTENTIAL SLOWDOWN - Get in Map (for string keys) -- WIP
            // USED OPTIMISATION: CollectionsMarshal.GetValueRefOrAddDefault to return a reference to an existing / newly created value in the dict
            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, stationName, out var exists);
            if (exists)
            {
                // The entry existed already, so update the existing ref
                measurement.Count++;
                measurement.Sum += parsedMeasurementValue;
                measurement.Max = Math.Max(measurement.Max, parsedMeasurementValue);
                measurement.Min = Math.Min(measurement.Min, parsedMeasurementValue);
            }
            else
            {
                // A new entry was created and the reference returned
                measurement.Count = 1;
                measurement.Sum = parsedMeasurementValue;
                measurement.Max = parsedMeasurementValue;
                measurement.Min = parsedMeasurementValue;
            }

            if (count++ != countResetThreshold || !debug) continue;
            
            Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s ({++auxIndex}/{iterations})");
            millionEntryWatch.Restart();
            count = 0;
        }

        return measurementsMap;
    }

    public Dictionary<string, ArrayList> ReadTextFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, ArrayList>(10000); // 10k unique station names, as per the spec
        
        // var delimiterSpan = ";".AsSpan();
        var delimiterSpan = ";".AsSpan();
        const int bufferSize = 1 * 1024 * 1024; // 1MB, {1 * 1024 * 1024, 1024 * 16}
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: bufferSize,
            detectEncodingFromByteOrderMarks: false);

        const int totalCount = 1000000000; // this won't change
        const int countResetThreshold = 10000000; // ~100 iterations
        var iterations = totalCount / countResetThreshold;
        
        var auxIndex = 0;
        var count = 0;
        
        // Declare these outside to minimise amount of allocations inside the read loop
        string? measurementLine;
        SplitTokens measurementTokens;
        string stationName;
        string stationMeasurement;
        char[][] measurementTokensChar2d;
        var millionEntryWatch = Stopwatch.StartNew();
        while (!reader.EndOfStream)
        {
            measurementLine = reader.ReadLine();
            // Retrieve tokens - Station name and measurement value (CUSTOM FOR LOOP SPLIT)
            /*measurementTokensChar2d = StringExtensions.SimpleSpanLoopBuilderSplit(measurementLine, ';');
            stationName = new string(measurementTokensChar2d[0]);
            stationMeasurement = new string(measurementTokensChar2d[1]);*/

            // Retrieve tokens - Station name and measurement value (CUSTOM SPAN SPLIT)
            measurementTokens = StringExtensions.SimpleSpanIndexSplit(measurementLine, delimiterSpan);
            stationName = measurementTokens.First;
            stationMeasurement = measurementTokens.Second;
            
            // Retrieve tokens - Station name and measurement value (STANDARD SPLIT)
            /*measurementTokens = measurementLine.Split(';');
            stationName = measurementTokens[0];
            stationMeasurement = measurementTokens[1];*/
            
            // Update result map - Add or append
            if (measurementsMap.TryGetValue(stationName, value: out _))
            {
                measurementsMap[stationName].Add(stationMeasurement);
            }
            else
            {
                measurementsMap.Add(stationName, [stationMeasurement]);
            }
            
            if (count++ == countResetThreshold)
            {
                Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s ({++auxIndex}/{iterations})");
                millionEntryWatch.Restart();
                count = 0;
            }
        }

        return measurementsMap;
    }

    public Dictionary<string, MeasurementData> ReadBytesFromFile(string filepath)
    {
        throw new NotImplementedException("not supported");
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}