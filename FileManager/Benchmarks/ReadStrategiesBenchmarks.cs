using System.Text;
using BenchmarkDotNet.Attributes;

namespace FileManager.Benchmarks;

[MemoryDiagnoser(false)]
public class ReadStrategiesBenchmarks
{
    private static readonly string Filepath = "../../../../../../../Data/measurements_subset.txt";
    private const int BufferSize = 10 * 1024 * 1024;
    private const int ChunkSize = 16;
    
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
    
    [Benchmark]
    public void Read_Bytes_BinaryReader_Iterator_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        reader.BaseStream.Position = 0;
        
        var index = 0;
        Span<byte> measurementLineBytes = stackalloc byte[120]; // 10 chars for temperature = 20 bytes + 100 bytes for station name = 120 bytes

        try
        {
            var inputByte = reader.ReadByte();
            measurementLineBytes[index++] = inputByte;
            while(inputByte > 0)
            {
                inputByte = reader.ReadByte();
                if (inputByte == 10)
                {
                    // newline, this would now pass the bytes in measurementLineBytes to the split code
                    measurementLineBytes.Clear();
                    continue;
                }
                measurementLineBytes[index++] = inputByte;
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    [Benchmark]
    public void Read_Bytes_BinaryReader_Chunk_LineByLine()
    {
        using var fileStream = new FileStream(Filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        reader.BaseStream.Position = 0;
        
        var index = 0;
        Span<byte> measurementLineBytes = stackalloc byte[120]; // 10 chars for temperature = 20 bytes + 100 bytes for station name = 120 bytes
        
        try
        {
            var inputByte = reader.ReadByte();
            measurementLineBytes[index++] = inputByte;
            while(inputByte > 0)
            {
                inputByte = reader.ReadByte();
                if (inputByte == 10)
                {
                    // newline, this would now pass the bytes in measurementLineBytes to the split code
                    measurementLineBytes.Clear();
                    continue;
                }
                measurementLineBytes[index++] = inputByte;
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}