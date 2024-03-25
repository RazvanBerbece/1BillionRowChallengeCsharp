namespace FileManager.Interfaces;

public interface IFileManager
{
    string ReadFromFile(string filepath);
    void WriteToFile(string filepath);
}