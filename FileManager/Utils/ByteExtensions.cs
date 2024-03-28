using System.Diagnostics;

namespace FileManager.Utils;

public static class ByteExtensions
{
    public static void SplitBytesOnByteValueAndAddToMap(byte[] byteArray, byte byteToFind, Dictionary<byte[], List<byte[]>> map)
    {
        var spanArray = byteArray.AsSpan();
        var delimiterIndex = Array.IndexOf(byteArray, byteToFind);
            
        // Map Updating
        var stationName = spanArray[..delimiterIndex].ToArray();
        var measurement = spanArray[(delimiterIndex + 1)..].ToArray();
        if (map.TryGetValue(stationName, value: out _))
        {
            map[stationName].Add(measurement);
        }
        else
        {
            map.Add(stationName, [measurement]);
        }
    }
    
    public static void SplitBatchOnByteValueAndAddToMap(byte[] byteArray, byte byteToFind, Dictionary<byte[], List<byte[]>> map)
    {
        var count = 0;
        var countResetThreshold = 1000000;
        var millionEntryWatch = Stopwatch.StartNew();
        
        // For each line in the batch
        var newline = Convert.ToByte('\n');
        var newlineIndex = Array.IndexOf(byteArray, newline);
        while (newlineIndex > 0)
        {
            var spanArray = byteArray.AsSpan()[..newlineIndex];
            var delimiterIndex = Array.IndexOf(spanArray.ToArray(), byteToFind);
            
            // Map Updating
            var stationName = spanArray[..delimiterIndex].ToArray();
            var measurement = spanArray[(delimiterIndex + 1)..newlineIndex].ToArray();
            if (map.TryGetValue(stationName, value: out _))
            {
                map[stationName].Add(measurement);
            }
            else
            {
                map.Add(stationName, [measurement]);
            }

            byteArray = byteArray[(newlineIndex + 1)..];
            newlineIndex = Array.IndexOf(byteArray, newline);
            
            count++;
            if (count == countResetThreshold)
            {
                Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s");
                millionEntryWatch.Restart();
                count = 0;
            }
        }
    }
}