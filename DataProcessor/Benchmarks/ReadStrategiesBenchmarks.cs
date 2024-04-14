using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace DataProcessor.Benchmarks;

[MemoryDiagnoser(false)]
public class ReadStrategiesBenchmarks
{
    private const string Filepath = "../../../../../../../Data/measurements_subset.txt";

    // Dynamic Params
    private const int BufferSize = 1024 * 10; // 10MB gave the best results

    /*[Params(16, 32, 64, 128, 512)]
    public int ChunkSize;*/
    
    [Benchmark]
    public void Read_Text_StreamReader_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: BufferSize,
            detectEncodingFromByteOrderMarks: false);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine().AsSpan();
        }
    }
    
    /*[Benchmark]
    public async Task Read_Text_StreamReader_Async_LineByLine()
    {
        await using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: BufferSize,
            detectEncodingFromByteOrderMarks: false);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
        }
    }*/
    
    /*[Benchmark]
    public void Read_Text_StreamReader_IteratorBuilder_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: BufferSize,
            detectEncodingFromByteOrderMarks: false);
        
        var measurementLineCharsIndex = 0;
        // 1 char for delimiter +
        // 10 chars max for recorded temperature +
        // 100 chars max for station name = 111 chars needed 
        Span<char> measurementLineChars = stackalloc char[111];
        
        while (!reader.EndOfStream)
        {
            var inputChar = reader.Read();
            if (inputChar is 10 or -1)
            {
                // newline or end of stream found, this would now pass the string in measurementLineChars to the split code
                // var tokens = split(measurementLineChars) etc.
                measurementLineChars.Clear();
                measurementLineCharsIndex = 0;
                continue;
            }
            measurementLineChars[measurementLineCharsIndex++] = (char)inputChar;
        }
    }*/
    
    /*[Benchmark]
    public void Read_Bytes_BinaryReader_IteratorBuilder_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        reader.BaseStream.Position = 0;
        
        var index = 0;
        // 2 bytes for delimiter +
        // 20 bytes max for recorded temperature +
        // 200 bytes max for station name = 222 bytes
        Span<byte> measurementLineBytes = stackalloc byte[222];

        try
        {
            while(reader.PeekChar() != -1)
            {
                var inputByte = reader.ReadByte();
                if (inputByte == 10)
                {
                    // newline found, this would now pass the bytes in measurementLineBytes to the split code
                    // var tokens = split(measurementLineBytes) etc.
                    measurementLineBytes.Clear();
                    index = 0;
                    continue;
                }
                measurementLineBytes[index++] = inputByte;
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }*/
    
    /*[Benchmark]
    public void Read_Bytes_BinaryReader_Chunks_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        var streamTotalLength = reader.BaseStream.Length;
        reader.BaseStream.Position = 0;
        
        Span<byte> finalChunk = stackalloc byte[64];
        Span<byte> measurementLineBytes = stackalloc byte[222];

        var foundNewline = false;
        while (reader.BaseStream.Position != streamTotalLength)
        {
            var buf = reader.ReadBytes(ChunkSize);
            if (buf[15] == 10)
            {
                Console.WriteLine(Encoding.Default.GetString(buf));
                // lucky edge case: this read an entire measurement line
                // send to split
                continue;
            }
        }
    }*/
    
    [Benchmark]
    public async Task Read_Bytes_PipelineReader_LineByLine()
    {
        await using var stream = new FileStream(Filepath, FileMode.Open);
        var reader = PipeReader.Create(stream, new StreamPipeReaderOptions(bufferSize: BufferSize));
        while (true)
        {
            var readResult = await reader.ReadAsync();
            var buffer = readResult.Buffer;
            while (TryReadLine(ref buffer, out var sequence))
            {
                // send to split code
                // Encoding.Default.GetString(sequence) is the text string of the measurement line
                // Console.WriteLine(Encoding.Default.GetString(sequence));
            }

            reader.AdvanceTo(buffer.Start, buffer.End);
            if (readResult.IsCompleted)
            {
                break;
            }
        }
    }
    
    // UTIL METHODS FOR THE VARIOUS APPROACHES
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var position = buffer.PositionOf((byte)'\n');
        if (position == null)
        {
            line = default;
            return false;
        }

        line = buffer.Slice(0, position.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

        return true;
    }
}