namespace DataProcessor.Extensions;

public static class RandomExtensionMethods
{
    public static float NextFloatRange(this Random random, float minNumber, float maxNumber)
    {
        return (float)random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }
}