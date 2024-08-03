using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using csFastFloat;
using DataProcessor.Domain;

namespace Benchmarks.Suites;

[MemoryDiagnoser]
public class OverallStrategyBenchmarks
{
    // Filepaths
    private const string DataFilepath = "../../../../../../../Data/measurements.txt";
    private const string DataSubsetFilepath = "../../../../../../../Data/measurements_subset.txt";

    // Constants
    private const byte DelimiterByte = 0x3B;
    private static ReadOnlySpan<char> Delimiter => [';'];
    private static ReadOnlySpan<byte> Newline => "\n"u8;
    private static readonly byte[] NewlineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

    // Dynamic Params
    private const int MapCapacity = 62851;
    private const int BufferSize = 5120;

    [Benchmark(Baseline = true)]
    public void Strategy_Naive()
    {
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

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

            // Add or append
            if (!measurementsMap.TryGetValue(stationName, out var measurement))
            {
                measurementsMap.Add(stationName, new MeasurementData
                {
                    Count = 1,
                    Min = parsedMeasurementValue,
                    Max = parsedMeasurementValue,
                    Sum = parsedMeasurementValue
                });
            }
            else
            {
                measurement.Count = +1;
                measurement.Sum += parsedMeasurementValue;
                measurement.Max = Math.Max(measurement.Max, parsedMeasurementValue);
                measurement.Min = Math.Min(measurement.Min, parsedMeasurementValue);
            }
        }
    }
    
    [Benchmark]
    public void Strategy_Naive_Spans()
    {
        ReadOnlySpan<char> delim = [';'];
        
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var lineSpan = line.AsSpan();
            
            var delimiterIndex = lineSpan.IndexOf(delim);
            var stationName = lineSpan[..delimiterIndex];
            var stationNameAsString = stationName.ToString(); // SLOWDOWN
            var stationMeasurement = lineSpan[(delimiterIndex + 1)..];

            var parsedMeasurementValue = float.Parse(stationMeasurement);

            // Add or append
            if (!measurementsMap.TryGetValue(stationNameAsString, out var measurement))
            {
                measurementsMap.Add(stationNameAsString, new MeasurementData
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
    
    [Benchmark]
    public void Strategy_Naive_Spans_LemireFloat()
    {
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

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

            // Add or append
            if (!measurementsMap.TryGetValue(stationNameAsString, out var measurement))
            {
                measurementsMap.Add(stationNameAsString, new MeasurementData
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
    
    [Benchmark]
    public void Strategy_Parallel_Naive_Spans_LemireFloat()
    {
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);
        
        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);
        
        Parallel.ForEach(LineGenerator(reader), currentLine =>
            {
                ProcessParallelMeasurement(ref measurementsMap, currentLine.AsSpan());
            } 
        );
    }
    
    /*[Benchmark]
    public void Strategy_Naive_Spans_LemireFloat_DictCollectionsMarshal()
    {
        ReadOnlySpan<char> delim = [';'];
        
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var lineSpan = line.AsSpan();
            
            var delimiterIndex = lineSpan.IndexOf(delim);
            var stationName = lineSpan[..delimiterIndex];
            var stationNameAsString = stationName.ToString(); // SLOWDOWN
            var stationMeasurement = lineSpan[(delimiterIndex + 1)..];

            var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);

            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, stationNameAsString, out var exists);
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
    }*/
    
    [Benchmark]
    public void Strategy_Naive_Spans_LemireFloat_DictCollectionsMarshal_ApplyMethod()
    {
        ReadOnlySpan<char> delim = [';'];
        
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var lineSpan = line.AsSpan();
            
            var delimiterIndex = lineSpan.IndexOf(delim);
            var stationName = lineSpan[..delimiterIndex];
            var stationNameAsString = stationName.ToString(); // SLOWDOWN
            var stationMeasurement = lineSpan[(delimiterIndex + 1)..];

            var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);

            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, stationNameAsString, out var exists);
            measurement.Apply(parsedMeasurementValue, !exists);
        }
    }
    
    [Benchmark]
    public void Strategy_Naive_Spans_LemireFloat_DictCollectionsMarshal_ApplyMethodInlined()
    {
        ReadOnlySpan<char> delim = [';'];
        
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            false);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var lineSpan = line.AsSpan();
            
            var delimiterIndex = lineSpan.IndexOf(delim);
            var stationName = lineSpan[..delimiterIndex];
            var stationNameAsString = stationName.ToString(); // SLOWDOWN
            var stationMeasurement = lineSpan[(delimiterIndex + 1)..];

            var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);

            ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, stationNameAsString, out var exists);
            measurement.Apply(parsedMeasurementValue, !exists);
        }
    }
    
    /*[Benchmark]
    public async Task Strategy_Naive_PipeReader()
    {
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<string, MeasurementData>(MapCapacity);

        await using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        var pipeReader = PipeReader.Create(fileStream);

        while (true)
        {
            var result = await pipeReader.ReadAsync();
            var buffer = result.Buffer;
            var sequencePos = ParseLines(ref measurementsMap, ref buffer);
            pipeReader.AdvanceTo(sequencePos, buffer.End);
            if (result.IsCompleted)
            {
                break;
            }
        }
        
        await pipeReader.CompleteAsync();
    }
    
    [Benchmark]
    public async Task Strategy_Naive_PipeReader_BytesKeys()
    {
        // NOTE: Remind of how important it is to use a running average instead of anything else (i.e list of floats, etc.)
        var measurementsMap = new Dictionary<byte[], MeasurementData>(MapCapacity);

        await using var fileStream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.SequentialScan);

        var pipeReader = PipeReader.Create(fileStream);

        while (true)
        {
            var result = await pipeReader.ReadAsync();
            var buffer = result.Buffer;
            var sequencePos = ParseLines_BytesKeysMap(ref measurementsMap, ref buffer);
            pipeReader.AdvanceTo(sequencePos, buffer.End);
            if (result.IsCompleted)
            {
                break;
            }
        }
        
        await pipeReader.CompleteAsync();
    }*/

    /*[Benchmark]
    public async Task Strategy_1()
    {
        await using var stream = new FileStream(
            DataSubsetFilepath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            BufferSize,
            FileOptions.RandomAccess);
        var reader = PipeReader.Create(stream, new StreamPipeReaderOptions());

        while (true)
        {
            var readResult = await reader.ReadAsync();
            var buffer = readResult.Buffer;

            // Parsing included in this function
            TryReadLineV2(ref buffer, out var pos);

            reader.AdvanceTo(pos, buffer.End);
            if (readResult.IsCompleted) break;
        }
    }*/

    // UTILS AND PRIVATES
    private static SequencePosition ParseLines(ref Dictionary<string, MeasurementData> map, ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);
        while (!reader.End)
        {
            if (!reader.TryReadToAny(out ReadOnlySpan<byte> line, Newline))
            {
                break; 
            }

            var delimiterIndex = line.IndexOf(DelimiterByte);
            
            var stationName = line[..delimiterIndex];
            var stationMeasurement = line[(delimiterIndex + 1)..];
            
            var stationNameAsString = Encoding.UTF8.GetString(stationName); // SLOWDOWN
            var measurementAsString = Encoding.UTF8.GetString(stationMeasurement); // SLOWDOWN

            var parsedMeasurementValue = float.Parse(measurementAsString);

            // Add or append
            if (!map.TryGetValue(stationNameAsString, out var measurement))
            {
                map.Add(stationNameAsString, new MeasurementData
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

        return reader.Position;
    }
    
    private static SequencePosition ParseLines_BytesKeysMap(ref Dictionary<byte[], MeasurementData> map, ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);
        Span<byte> floatContainer = stackalloc byte[4];
        while (!reader.End)
        {
            if (!reader.TryReadToAny(out ReadOnlySpan<byte> line, Newline))
            {
                break; 
            }

            var delimiterIndex = line.IndexOf(DelimiterByte);
            
            var stationName = line[..delimiterIndex];
            var stationMeasurement = line[(delimiterIndex + 1)..];
            
            // Minimise amount of copies by only doing them when the span is less than 
            // 4 bytes for a single precision float
            if (stationMeasurement.Length < 4)
            {
                stationMeasurement.CopyTo(floatContainer);
            }

            var stationNameArray = stationName.ToArray();

            var parsedMeasurementValue = BitConverter.ToSingle(floatContainer);

            // Add or append
            if (!map.TryGetValue(stationNameArray, out var measurement))
            {
                map.Add(stationNameArray, new MeasurementData
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

        return reader.Position;
    }
    
    private static void TryReadLineV2(ref ReadOnlySequence<byte> buffer, out SequencePosition pos)
    {
        var measurementsMap = new Dictionary<byte[], MeasurementData>(MapCapacity);

        var newline = NewlineBytes.AsSpan();

        var reader = new SequenceReader<byte>(buffer);

        while (!reader.End)
        {
            if (!reader.TryReadToAny(out ReadOnlySpan<byte> inputLine, newline))
                // no more newlines found
                break;

            // split input line by delimiter to get station name and measurement
            var delimiterIndex = inputLine.IndexOf(DelimiterByte);

            var stationName = inputLine[(delimiterIndex + 1)..].ToArray();
            var stationMeasurement = inputLine[..delimiterIndex];
            _ = FastFloatParser.TryParseFloat(stationMeasurement, out var floatVal);
            ref var measurement =
                ref CollectionsMarshal.GetValueRefOrAddDefault(measurementsMap, stationName, out var exists);
            if (exists)
            {
                // The entry existed already, so update the existing ref
                measurement.Count++;
                measurement.Sum += floatVal;
                measurement.Max = Math.Max(measurement.Max, floatVal);
                measurement.Min = Math.Min(measurement.Min, floatVal);
            }
            else
            {
                // A new entry was created and the reference returned
                measurement.Count = 1;
                measurement.Sum = floatVal;
                measurement.Max = floatVal;
                measurement.Min = floatVal;
            }
        }

        pos = reader.Position;
    }

    private static void ProcessParallelMeasurement(ref Dictionary<string, MeasurementData> map, ReadOnlySpan<char> input)
    {
        var delimiterIndex = input.IndexOf(Delimiter);
        var stationName = input[..delimiterIndex];
        var stationNameAsString = stationName.ToString(); // SLOWDOWN
        var stationMeasurement = input[(delimiterIndex + 1)..];
                
        var parsedMeasurementValue = FastFloatParser.ParseFloat(stationMeasurement);
                
        ref var measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(map, stationNameAsString, out var exists);
        measurement.Apply(parsedMeasurementValue, !exists);
    }
    
    private static IEnumerable<string> LineGenerator(StreamReader sr)
    {
        while (sr.ReadLine() is { } line)
        {
            yield return line;
        }
    }
}