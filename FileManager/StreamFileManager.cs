using System.Text;
using FileManager.Interfaces;

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
    
    public Dictionary<string, List<string>> ReadFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, List<string>>();
        
        using var reader = new StreamReader(
            new FileStream(filepath, FileMode.Open), 
            bufferSize: 8192);
        
        while (reader.ReadLine() is { } line)
        {
            var measurementTokens = line.Split(";");
            var stationName = measurementTokens[0];
            var stationMeasurement = measurementTokens[1];
                
            // Add or append
            if (measurementsMap.TryGetValue(stationName, value: out _))
            {
                measurementsMap[stationName].Add(stationMeasurement);
            }
            else
            {
                measurementsMap.Add(stationName, [stationMeasurement]);
            }
        }

        return measurementsMap;
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}