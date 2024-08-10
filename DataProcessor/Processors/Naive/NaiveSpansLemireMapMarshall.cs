using System.Runtime.InteropServices;
using System.Text;
using csFastFloat;
using DataProcessor.Domain;

namespace DataProcessor.Processors.Naive;

public class NaiveSpansLemireMapMarshall(string dataFilepath)
{
    private static ReadOnlySpan<char> Delimiter => [';'];

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
            var lineSpan = line.AsSpan();

            var delimiterIndex = lineSpan.IndexOf(Delimiter);
            var stationName = lineSpan[..delimiterIndex];
            var stationNameAsString = stationName.ToString(); // SLOWDOWN
            var stationMeasurement = lineSpan[(delimiterIndex + 1)..];

            var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);

            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(MeasurementsMap, stationNameAsString, out var exists);
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
        }
    }
}