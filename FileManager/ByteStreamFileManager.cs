using System.Collections;
using System.Text;
using FileManager.Domain;
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

    public Dictionary<string, MeasurementData> ReadTextFromFileInCustomStruct(string filepath)
    {
        var measurementsMap = new Dictionary<string, MeasurementData>(10000); // 10k unique station names, as per the spec
        
        var fileStream = new FileStream(filepath, FileMode.Open);
        using var reader = new StreamReader(fileStream, new UTF8Encoding());

        var measurementLine = "";
        const int bufferSize = 16; // i.e chunk size
        var readBytes = new char[bufferSize];
        while (!reader.EndOfStream)
        {
           reader.Read();
        }

        return measurementsMap;
    }

    public Dictionary<string, ArrayList> ReadTextFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, ArrayList>(10000); // 10k unique station names, as per the spec
        
        var fileStream = new FileStream(filepath, FileMode.Open);
        using var reader = new BinaryReader(fileStream, new UTF8Encoding());
        
        var semicolon = Convert.ToByte(';');
        const int chunkSize = 1 * 1024 * 1024; // read in 1MB chunks
        // var numBatches = fileStream.Length / chunkSize;
        var chunk = reader.ReadBytes(chunkSize);
        
        while(chunk.Length > 0)
        {
            ByteExtensions.SimpleFixedArrayLoopBuilderSplit(chunk, semicolon, measurementsMap);
            chunk = reader.ReadBytes(chunkSize);
        }

        return measurementsMap;
    }

    public Dictionary<string, MeasurementData> ReadBytesFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<string, MeasurementData>(10000); // 10k unique station names, as per the spec
        
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(
            fileStream, 
            new UTF8Encoding());
        
        var semicolon = Convert.ToByte(';');
        const int chunkSize = 1 * 1024 * 1024; // read in 1MB chunks
        // var numBatches = fileStream.Length / chunkSize;
        var chunk = reader.ReadBytes(chunkSize);
        
        while(chunk.Length > 0)
        {
            //ByteExtensions.SplitBatchOnByteValueAndAddToMap(chunk, semicolon, measurementsMap);
            chunk = reader.ReadBytes(chunkSize);
        }

        return measurementsMap;
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}