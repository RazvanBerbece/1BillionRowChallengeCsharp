using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FileManager.Utils;

public static class ByteExtensions
{
    public static unsafe void SimpleFixedArrayLoopBuilderSplit(byte[] byteArray, byte delimiter, Dictionary<string, ArrayList> map)
    {
        const int totalCount = 1000000000; // this won't change
        var count = 0;
        var countResetThreshold = 100000;
        var iterations = totalCount / countResetThreshold;
        
        var auxIndex = 0;

        var posIndex = 0;
        var foundDelimiter = false;
        var firstIndex = 0;
        var first = stackalloc byte[100]; // name
        var secondIndex = 0;
        var second = stackalloc byte[16]; // measurement e.g -99.9

        fixed (byte* pSource = byteArray)
        {
            var newline = Convert.ToByte('\n');
            var newlineIndex = 0;
            
            var millionEntryWatch = Stopwatch.StartNew();
            while (newlineIndex != -1)
            {
                for (var i = posIndex; i < newlineIndex - 1; ++i)
                {
                    if (pSource[i] == delimiter)
                    {
                        foundDelimiter = true;
                        continue;
                    }
                
                    switch (foundDelimiter)
                    {
                        case false:
                            first[firstIndex++] = pSource[i];
                            break;
                        case true:
                            second[secondIndex++] = pSource[i];
                            break;
                    }
                }
            
                newlineIndex = Array.IndexOf(byteArray, newline);
            
                if (count++ == countResetThreshold)
                {
                    Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s ({++auxIndex}/{iterations})");
                    millionEntryWatch.Restart();
                    count = 0;
                }
            }
        }
        
        fixed (byte* pSource = byteArray)
        {

            var firstStr = Marshal.PtrToStringUTF8(*first);
            var secondStr = Marshal.PtrToStringUTF8(*second);
            if (map.TryGetValue(firstStr, value: out _))
            {
                map[firstStr].Add(secondStr);
            }
            else
            {
                map.Add(firstStr, [secondStr]);
            }
        }
    }
    
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