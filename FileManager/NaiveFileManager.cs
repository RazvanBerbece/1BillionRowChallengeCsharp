using FileManager.Interfaces;

namespace FileManager;

/// <summary>
/// Naive implementation of a file manager that provides methods to read data from a file and to write to one.
/// </summary>
public class NaiveFileManager: IFileManager
{
    public NaiveFileManager()
    {
    }
    
    public string ReadFromFile(string filepath)
    {
        throw new NotImplementedException();
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}