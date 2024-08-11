using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using csFastFloat;
using DataProcessor.Domain;

namespace DataProcessor.Processors.Bytes;

public class BufferedBytesSpansLemireEnhanced(string dataFilepath)
{
    private const byte Newline = 10;
    private const byte Delimiter = 59; // ;

    // Dynamic Params
    private const int MapCapacity = 62851;
    private const int BufferSize = 8192;

    // compared to BytesSpansLemire, it seems that arrays are keys are S L O W
    public readonly Dictionary<string, MeasurementData> MeasurementsMap = new(MapCapacity);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Process()
    {
        using var fileStream = new FileStream(
            dataFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        int readBytes;
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        var startIndex = 0;

        Span<byte> leftover = stackalloc byte[0];

        while ((readBytes = fileStream.Read(buffer, startIndex, BufferSize - startIndex)) > 0)
        {
            var dataSpan = buffer.AsSpan(0, readBytes + startIndex);
            var lineStart = 0;

            while (true)
            {
                var newlineIndex = dataSpan[lineStart..].IndexOf(Newline);
                if (newlineIndex == -1)
                {
                    // No complete line found, save the leftover for the next batch
                    leftover = dataSpan[lineStart..].ToArray();
                    break;
                }

                // Process the line
                var line = dataSpan.Slice(lineStart, newlineIndex);
                var delimiterIndex = line.IndexOf(Delimiter);

                // Get station name and temperature
                var stationNameSpan = line[..delimiterIndex];
                var temperatureSpan = line[(delimiterIndex + 1)..];

                // TODO: Try and do this without any allocation ? Requires custom dict keys
                var stationName = Encoding.UTF8.GetString(stationNameSpan);
                var parsedMeasurementValue =
                    FastFloatParser.ParseFloat(temperatureSpan); // TODO: can this be even faster ? int approach ?

                if (!MeasurementsMap.TryGetValue(stationName, out var measurement))
                {
                    MeasurementsMap[stationName] = new MeasurementData
                    {
                        Count = 1,
                        Min = parsedMeasurementValue,
                        Max = parsedMeasurementValue,
                        Sum = parsedMeasurementValue
                    };
                }
                else
                {
                    measurement.Count++;
                    measurement.Sum += parsedMeasurementValue;
                    measurement.Max = Math.Max(measurement.Max, parsedMeasurementValue);
                    measurement.Min = Math.Min(measurement.Min, parsedMeasurementValue);
                }

                lineStart += newlineIndex + 1;
            }
            
            if (leftover.Length == 0)
            {
                startIndex = 0;
            }
            else
            {
                leftover.CopyTo(buffer);
                startIndex = leftover.Length;
            }
        }
        
        ArrayPool<byte>.Shared.Return(buffer);
    }
}