using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using FileManager.Domain;

namespace FileManager.Benchmarks;

[MemoryDiagnoser(false)]
public class ReadStrategiesBenchmarks
{
    
    private static readonly string Filepath = "../../../../../../../Data/measurements_subset.txt";
    private const int BufferSize = 10 * 1024 * 1024;
    
    [Benchmark]
    public void Read_Text_StreamReader_LineByLine()
    {
        var fileStream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(
            fileStream, 
            encoding: Encoding.UTF8, 
            bufferSize: BufferSize,
            detectEncodingFromByteOrderMarks: false);
        
        while (!reader.EndOfStream)
        {
            _ = reader.ReadLine();
        }
    }
}