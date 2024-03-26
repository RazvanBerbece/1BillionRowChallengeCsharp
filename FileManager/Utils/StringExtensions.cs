namespace FileManager.Utils;

public static class StringExtensions
{
    public static string[] SimpleSplit(string str, char delimiter)
    {
        ReadOnlySpan<char> inputSpan = str;
        var delimiterIndex = str.IndexOf(delimiter);
        var first = inputSpan[..delimiterIndex];
        var second = inputSpan[(delimiterIndex + 1)..];
        return [first.ToString(), second.ToString()];
    }
}