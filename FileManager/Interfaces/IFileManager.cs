using System.Collections;

namespace FileManager.Interfaces;

public interface IFileManager
{
    Dictionary<string, ArrayList> ReadTextFromFile(string filepath);
    Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath);
    void WriteToFile(string filepath);
}