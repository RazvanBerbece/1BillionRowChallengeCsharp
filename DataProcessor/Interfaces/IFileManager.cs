using System.Collections;
using DataProcessor.Domain;

namespace DataProcessor.Interfaces;

public interface IFileManager
{
    Dictionary<string, MeasurementData> ReadTextFromFileInCustomStruct(string filepath);
    Dictionary<string, ArrayList> ReadTextFromFile(string filepath);
    Dictionary<string, MeasurementData> ReadBytesFromFile(string filepath);
    void WriteToFile(string filepath);
}