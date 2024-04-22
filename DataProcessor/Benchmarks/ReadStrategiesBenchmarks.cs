using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace DataProcessor.Benchmarks;

[MemoryDiagnoser]
public class ReadStrategiesBenchmarks
{
    private const string DataFilepath = "../../../../../../../Data/measurements.txt";
    private const string DataSubsetFilepath = "../../../../../../../Data/measurements_subset.txt";

    // Dynamic Params
    private const int BufferSize = 5120; // 5120 bytes gave the best results so far

    /*[Params(16, 32, 64, 128, 512)]
    public int ChunkSize;*/
    
    [Benchmark]
    public void Read_Text_StreamReader_LineByLine()
    {
        using var fileStream = new FileStream(DataSubsetFilepath, FileMode.Open, FileAccess.Read);
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
    public void Read_Text_Parallel_ForEach_LineByLine()
    {
        // This approach is probably only suitable in the full dataset case
        Parallel.ForEach(File.ReadLines(DataFilepath), line =>
        {
            var span = line.AsSpan();
            // Split code here
        });
    }*/
    
    /*[Benchmark]
    public void Read_Text_MmapFile_LineByLine()
    {
        using var mmf = MemoryMappedFile.CreateFromFile(DataFilepath);
        using var mmvStream = mmf.CreateViewStream();
        using var sr = new StreamReader(mmvStream, bufferSize: BufferSize);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine().AsSpan();
        }
    }*/
    
    /*[Benchmark]
    public async Task Read_Text_StreamReader_Async_LineByLine()
    {
        await using var fileStream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);
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
        using var fileStream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);
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
        using var fileStream = new FileStream(DataSubsetFilepath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        reader.BaseStream.Position = 0;
        
        var index = 0;
        // 2 bytes for delimiter +
        // 20 bytes max for recorded temperature +
        // 200 bytes max for station name (100 characters) = 222 bytes
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
        using var fileStream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);
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
        await using var stream = new FileStream(DataSubsetFilepath, FileMode.Open, FileAccess.Read);
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
    
    [Benchmark]
    public async Task Read_Bytes_PipelineReader_LineByLine_v2()
    {
        await using var stream = new FileStream(DataSubsetFilepath, FileMode.Open, FileAccess.Read);
        var reader = PipeReader.Create(stream, new StreamPipeReaderOptions(bufferSize: BufferSize));
        while (true)
        {
            var readResult = await reader.ReadAsync();
            var buffer = readResult.Buffer;
            
            // Parsing included in this function
            TryReadLineV2(ref buffer, out var pos);

            reader.AdvanceTo(pos, buffer.End);
            if (readResult.IsCompleted)
            {
                break;
            }
        }
    }
    
    [Benchmark]
    public void Read_Bytes_Chunks_Seek_To_Newlines()
    {
        using var stream = new FileStream(DataSubsetFilepath, FileMode.Open, FileAccess.Read);
        var streamSize = stream.Length;
        stream.Position = 0;
        
        // \n as span
        const byte newlineDelimiter = (byte)'\n';
        
        // allocate 222 / 223 bytes in order to ensure the lazy capture of a newline (abusing specs here a bit)
        Span<byte> buffer = stackalloc byte[223];

        while (stream.Read(buffer) > 0)
        {
            // 223 bytes in buffer
            
            // look for next nearest newline from buffer start to end
            var nextNewlinePosInChunk = buffer.IndexOf(newlineDelimiter);
            
            // measurementLine available to split here
            var measurementLine = buffer[..nextNewlinePosInChunk];
            // Console.WriteLine(Encoding.Default.GetString(measurementLine));
            
            // if EOF, then break early
            if (stream.Position == streamSize)
            {
                break;
            }
            
            // stream.Position -= 222 - nextNewlinePosInChunk;
            stream.Seek(
                stream.Position - (222 - nextNewlinePosInChunk),  // 223 - nextNewlinePosInChunk - 1 
                SeekOrigin.Begin);
            /*
             Drop the call to Seek, change the position directly (improved time by ~1ms)
             stream.Seek(
                stream.Position - (222 - nextNewlinePosInChunk),  // 223 - nextNewlinePosInChunk - 1 
                SeekOrigin.Begin);*/
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TryReadLineV2(ref ReadOnlySequence<byte> buffer, out SequencePosition pos)
    {
        var newline = Encoding.UTF8.GetBytes(Environment.NewLine).AsSpan();
        
        var reader = new SequenceReader<byte>(buffer);

        while (!reader.End)
        {
            if (!reader.TryReadToAny(out ReadOnlySpan<byte> inputLine, newline))
            {
                // no more newlines found
                break;
            }
            
            // measurementLine available to split here
            // Console.WriteLine(Encoding.Default.GetString(inputLine));
        }

        pos = reader.Position;
    }
}