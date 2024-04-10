using System.Collections;
using FileManager.Domain;
using FileManager.Interfaces;

namespace FileManager;

public class ByteSpanStreamFileManager: IFileManager
{
    public Dictionary<ByteSpan, MeasurementData> ReadBytesFromFileWithCustomSpanKeys(string filepath)
    {
        throw new NotImplementedException();
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
        // Read byte chunks of size N KB / MB ? from file
        
        // Split chunk on newline byte
        // Ensure chunk goes all the way to a newline to avoid chunk endings happening in the middle of a line ?
        
        // Foreach token (1 measurement data line)
        // Split on delimiter (;) byte

        return null;
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}