namespace FileManager.Utils;

public static class StringExtensions
{
    public static string[] SimpleSpanSplit(string str, char delimiter)
    {
        var span = str.AsSpan();
        
        // Retrieve tokens
        var delimiterIndex = span.IndexOf(delimiter);
        var first = span[..delimiterIndex].ToArray();
        var second = span[(delimiterIndex + 1)..].ToArray();

        return [new string(first), new string(second)];
    }
}