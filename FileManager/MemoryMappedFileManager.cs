/*using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;
using FileManager.Interfaces;
using FileManager.Utils;

namespace FileManager;

/// <summary>
/// Custom implementation of a file manager that provides methods to read data from a file and to write to one using
/// more efficient streams.
/// </summary>
public class MemoryMappedFileManager: IFileManager
{
    public MemoryMappedFileManager()
    {
    }
    
    public Dictionary<string, string[]> ReadTextFromFile(string filepath)
    {
        throw new NotImplementedException("not supported");
    }

    public Dictionary<byte[], List<byte[]>> ReadBytesFromFile(string filepath)
    {
        var measurementsMap = new Dictionary<byte[], List<byte[]>>(10000); // 10k unique station names, as per the spec
        
        const int bufferSize = 1024 * 1024 * 10;
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fileStream, bufferSize: bufferSize);
        
        // memory map the input file
        var mmf = MemoryMappedFile.CreateFromFile(filepath, FileMode.Open, null, fileStream.Length);
        var accessor = mmf.CreateViewAccessor();

        const int totalCount = 1000000000; // this won't change
        const int countResetThreshold = 10000000;
        var iterations = totalCount / countResetThreshold;
        
        var auxIndex = 0;
        var count = 0;
        
        // Declare these outside to minimise amount of allocations inside the read loop
        var newlineCode = Convert.ToByte('\n');
        var delimiterCode = Convert.ToByte('\n');
        var measurementBytes = new byte[512];
        var positionInBuffer = 0;
        accessor.ReadArray(positionInBuffer, measurementBytes, 0, 512);
        var newlineIndex = Array.IndexOf(measurementBytes, newlineCode);
        var millionEntryWatch = Stopwatch.StartNew();
        while (newlineIndex != -1)
        {
            accessor.ReadArray(positionInBuffer, measurementBytes, 0, newlineIndex);
            ByteExtensions.SplitBytesOnByteValueAndAddToMap(measurementBytes, delimiterCode, measurementsMap);

            positionInBuffer = newlineIndex;
            newlineIndex = Array.IndexOf(measurementBytes[(newlineIndex + 1)..], newlineCode);
            
            if (count++ == countResetThreshold)
            {
                Console.WriteLine($"Processed {countResetThreshold} inputs in {millionEntryWatch.Elapsed.TotalSeconds}s ({++auxIndex}/{iterations})");
                millionEntryWatch.Restart();
                count = 0;
            }
        }

        return measurementsMap;
    }

    public void WriteToFile(string filepath)
    {
        throw new NotImplementedException();
    }
}*/