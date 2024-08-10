using System.Text;
using DataProcessor.Domain;

namespace DataProcessor.Processors.Naive;

public class Naive(string dataFilepath)
{
    // Dynamic Params
    private const int MapCapacity = 62851;
    private const int BufferSize = 5120;
    private const FileOptions FileScanType = FileOptions.SequentialScan;

    public readonly Dictionary<string, MeasurementData> MeasurementsMap = new(MapCapacity);

    public void Process()
    {
        using var fileStream = new FileStream(
            dataFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileScanType);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            var measurementTokens = line.Split(';');
            var stationName = measurementTokens[0];
            var stationMeasurement = measurementTokens[1];
            
            var parsedMeasurementValue = float.Parse(stationMeasurement);
            
            if (!MeasurementsMap.TryGetValue(stationName, out var measurement))
            {
                MeasurementsMap.Add(stationName, new MeasurementData
                {
                    Count = 1,
                    Min = parsedMeasurementValue,
                    Max = parsedMeasurementValue,
                    Sum = parsedMeasurementValue
                });
            }
            else
            {
                measurement.Count++;
                measurement.Sum += parsedMeasurementValue;
                measurement.Max = Math.Max(measurement.Max, parsedMeasurementValue);
                measurement.Min = Math.Min(measurement.Min, parsedMeasurementValue);
            }
        }
    }
}