using System.Diagnostics;
using System.Text;
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
    
    public Dictionary<string, List<string>> ReadTextFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, List<string>>(10000); // 10k unique station names, as per the spec
        
        var delimiterSpan = ";".AsSpan();
        const int bufferSize = 1024 * 1024 * 15;
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fileStream, bufferSize: bufferSize);
        
        var count = 0;
        var countResetThreshold = 10000000; // ~100 iterations
        
        // Declare these outside to minimise amount of allocations inside the read loop
        string? measurementLine;
        string[] measurementTokens;
        string stationName;
        string stationMeasurement;
        char[][] measurementTokensChar2d;
        var millionEntryWatch = Stopwatch.StartNew();
        while ((measurementLine = reader.ReadLine()) != null)
        {
            // Retrieve tokens - Station name and measurement value (CUSTOM FOR LOOP SPLIT)
            /*measurementTokensChar2d = StringExtensions.SimpleSpanLoopBuilderSplit(measurementLine, ';');
            stationName = new string(measurementTokensChar2d[0]);
            stationMeasurement = new string(measurementTokensChar2d[1]);*/

            // Retrieve tokens - Station name and measurement value (CUSTOM SPAN SPLIT)
            /*measurementTokens = StringExtensions.SimpleSpanIndexSplit(measurementLine, delimiterSpan);
            stationName = measurementTokens[0];
            stationMeasurement = measurementTokens[1];*/
            
            // Retrieve tokens - Station name and measurement value (STANDARD SPLIT)
            measurementTokens = measurementLine.Split(';');
            stationName = measurementTokens[0];
            stationMeasurement = measurementTokens[1];
            
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
                Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s");
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
}