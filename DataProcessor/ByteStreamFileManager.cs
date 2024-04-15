using System.Collections;
using DataProcessor.Domain;
using DataProcessor.Interfaces;

namespace DataProcessor;

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
        throw new NotImplementedException();
    }

    public Dictionary<string, ArrayList> ReadTextFromFile(string filepath)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, MeasurementData> ReadBytesFromFile(string filepath)
    {
        throw new NotImplementedException();
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}