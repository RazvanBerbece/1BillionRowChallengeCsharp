namespace FileManager.Domain;

public struct MeasurementData
{
    public string[] Measurements;

    public MeasurementData()
    {
        Measurements = new string[10000];
    }
}