/*using DataProcessor.Interfaces;

namespace DataProcessor;

/// <summary>
/// Naive implementation of a file manager that provides methods to read data from a file and to write to one.
/// </summary>
public class NaiveFileManager: IFileManager
{
    public NaiveFileManager()
    {
    }
    
    public Dictionary<string, List<string>> ReadFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, List<string>>();
        
        var measurementLines = File.ReadLines(filepath);
        foreach (var measurementLine in measurementLines)
        {
            var measurementTokens = measurementLine.Split(";");
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
}*/