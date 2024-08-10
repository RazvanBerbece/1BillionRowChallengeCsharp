using System.Text;
using csFastFloat;
using DataProcessor.Domain;

namespace DataProcessor.Processors.Bytes;

public class BytesSpansLemire(string dataFilepath)
{
    private const byte Newline = 10;
    private const byte Delimiter = 59; // ;

    // Dynamic Params
    private const int MapCapacity = 62851;

    public readonly Dictionary<string, MeasurementData> MeasurementsMap = new(MapCapacity);

    public void Process()
    {
        using var fileStream = new FileStream(
            dataFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);
        
        var count = 0;
        var buffer = new byte[200];
        int readByte;
        while ((readByte = fileStream.ReadByte()) != -1)
        {
            if (readByte == Newline)
            {
                // Complete line in buffer, so process it
                var bytesSpan = buffer.AsSpan();

                // Find the delimiter in the current buffer (i.e. ;)
                var delimiterIndex = bytesSpan.IndexOf(Delimiter);

                // Get the station name
                var stationNameBytes = Encoding.UTF8.GetString(bytesSpan[..delimiterIndex]);

                // Get the temperature measurement and convert to float
                var stationMeasurement = bytesSpan[(delimiterIndex + 1)..];
                var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);

                if (!MeasurementsMap.TryGetValue(stationNameBytes, out var measurement))
                {
                    MeasurementsMap.Add(stationNameBytes, new MeasurementData
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
                
                Array.Clear(buffer);
                count = 0;
                continue;
            }

            buffer[count++] = Convert.ToByte(readByte);
        }
    }
}