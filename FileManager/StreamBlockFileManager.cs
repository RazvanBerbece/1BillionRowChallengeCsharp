/*using System.Diagnostics;
using System.Text;
using FileManager.Interfaces;
using FileManager.Utils;

namespace FileManager;

/// <summary>
/// Custom implementation of a file manager that provides methods to read data from a file and to write to one using
/// more efficient streams.
/// </summary>
public class StreamBlockFileManager: IFileManager
{
    public StreamBlockFileManager()
    {
    }
    
    public Dictionary<string, string[]> ReadTextFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, string[]>(10000); // 10k unique station names, as per the spec
        
        var delimiterSpan = ";".AsSpan();
        const int bufferSize = 1024 * 1024 * 10;
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fileStream, bufferSize: bufferSize);

        const int totalCount = 1000000000; // this won't change
        const int countResetThreshold = 10000000; // ~100 iterations
        var iterations = totalCount / countResetThreshold;
        
        var auxIndex = 0;
        var count = 0;
        
        // Declare these outside to minimise amount of allocations inside the read loop
        string? measurementLine;
        string[] measurementTokens;
        string stationName;
        string stationMeasurement;
        char[] buffer = new char[]{ };
        reader.ReadBlock(buffer, 0, 64); // an estimation for now
        int newlineIndex = buffer.AsSpan().IndexOf('\n');
        int positionIndex = 0;
        int delimiterIndex;
        var millionEntryWatch = Stopwatch.StartNew();
        while (newlineIndex != -1)
        {
            // Retrieve tokens - Station name and measurement value (CUSTOM SPAN SPLIT)
            reader.ReadBlock(buffer, 0, newlineIndex); // an estimation for now
            var bufferSpan = buffer.AsSpan();
            delimiterIndex = bufferSpan.IndexOf(';');
            stationName = bufferSpan[..delimiterIndex].ToString();
            stationMeasurement = bufferSpan[(delimiterIndex + 1)..].ToString();
            
            // Retrieve tokens - Station name and measurement value (STANDARD SPLIT)
            /*measurementTokens = measurementLine.Split(';');
            stationName = measurementTokens[0];
            stationMeasurement = measurementTokens[1];#1#
            
            // Update result map - Add or append
            if (measurementsMap.TryGetValue(stationName, value: out _))
            {
                var newIndex = measurementsMap[stationName].Length;
                measurementsMap[stationName][newIndex] = stationMeasurement;
            }
            else
            {
                measurementsMap.Add(stationName, [stationMeasurement]);
            }
            
            newlineIndex = bufferSpan[(newlineIndex + 1)..].IndexOf('\n');
            positionIndex = newlineIndex;
            
            if (count++ == countResetThreshold)
            {
                Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s ({++auxIndex}/{iterations})");
                millionEntryWatch.Restart();
                count = 0;
            }
        }

        return measurementsMap;
    }

    public Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath)
    {
        throw new NotImplementedException("not supported");
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}*/