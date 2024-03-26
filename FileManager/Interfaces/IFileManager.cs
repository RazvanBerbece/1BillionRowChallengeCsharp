namespace FileManager.Interfaces;

public interface IFileManager
{
    Dictionary<string, List<string>> ReadFromFile(string filepath);
    void WriteToFile(string filepath);
}