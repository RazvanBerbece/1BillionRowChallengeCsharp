using System.Collections;
using FileManager.Domain;

namespace FileManager.Interfaces;

public interface IFileManager
{
    Dictionary<string, MeasurementData> ReadTextFromFileInCustomStruct(string filepath);
    Dictionary<string, ArrayList> ReadTextFromFile(string filepath);
    Dictionary<string, MeasurementData> ReadBytesFromFile(string filepath);
    void WriteToFile(string filepath);
}