namespace FileManager.Interfaces;

public interface IFileManager
{
    Dictionary<string, List<string>> ReadTextFromFile(string filepath);
    Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath);
    void WriteToFile(string filepath);
}