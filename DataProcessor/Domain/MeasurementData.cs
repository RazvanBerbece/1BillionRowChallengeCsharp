using System.Runtime.CompilerServices;

namespace DataProcessor.Domain;

public struct MeasurementData()
{
    public float Sum = 0;
    public float Min = 0;
    public float Max = 0;
    public uint Count = 0;

    public float Average => Sum / Count;
    
    public void Apply(float value, bool isFirst)
    {
        if (value > Max || isFirst) Max = value;
        if (value < Min || isFirst) Min = value;
        Sum += value;
        Count++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyInlined(float value, bool isFirst)
    {
        if (value > Max || isFirst) Max = value;
        if (value < Min || isFirst) Min = value;
        Sum += value;
        Count++;
    }
}