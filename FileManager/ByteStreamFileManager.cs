/*using System.Text;
using FileManager.Interfaces;
using FileManager.Utils;

namespace FileManager;

/// <summary>
/// Custom implementation of a file manager that provides methods to read data from a file and to write to one using
/// more efficient streams.
/// </summary>
public class ByteStreamFileManager: IFileManager
{
    public ByteStreamFileManager()
    {
    }

    public Dictionary<string, string[]> ReadTextFromFile(string filepath)
    {
        throw new NotImplementedException("not supported");
    }

    public Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<byte[], List<byte[]>>(10000); // 10k unique station names, as per the spec
        
        var fileStream = new FileStream(filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        
        var semicolon = Convert.ToByte(';');
        const int chunkSize = 1024 * 1024 * 10; // read in 10MB chunks
        // var numBatches = fileStream.Length / chunkSize;
        var chunk = reader.ReadBytes(chunkSize);
        
        while(chunk.Length > 0)
        {
            ByteExtensions.SplitBatchOnByteValueAndAddToMap(chunk, semicolon, measurementsMap);
            chunk = reader.ReadBytes(chunkSize);
        }

        return measurementsMap;
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}*/