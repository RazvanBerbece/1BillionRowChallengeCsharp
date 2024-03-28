using System.Collections;
using FileManager.Domain;

namespace FileManager.Interfaces;

public interface IFileManager
{
    Dictionary<string, MeasurementData> ReadTextFromFileInCustomStruct(string filepath);
    Dictionary<string, ArrayList> ReadTextFromFile(string filepath);
    Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath);
    void WriteToFile(string filepath);
}