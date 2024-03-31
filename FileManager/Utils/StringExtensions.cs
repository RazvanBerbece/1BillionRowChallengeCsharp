using FileManager.Domain;

namespace FileManager.Utils;

public static class StringExtensions
{
    public static char[][] SimpleSpanLoopBuilderSplit(string str, char delimiter)
    {
        var span = str.AsSpan();
        
        // Retrieve tokens
        var foundDelimiter = false;
        var firstIndex = 0;
        var first = new char[100];
        var secondIndex = 0;
        var second = new char[5];
        foreach (var t in span)
        {
            if (t == delimiter)
            {
                foundDelimiter = true;
                continue;
            }

            switch (foundDelimiter)
            {
                case false:
                    first[firstIndex++] = t;
                    break;
                case true:
                    second[secondIndex++] = t;
                    break;
            }
        }

        return [first, second];
    }
    
    public static SplitTokens SimpleSpanIndexSplit(string str, ReadOnlySpan<char> delimiter)
    {
        var span = str.AsSpan();
        
        // Retrieve tokens
        var delimiterIndex = span.IndexOf(delimiter, StringComparison.Ordinal);
        var first = span[..delimiterIndex];
        var second = span[(delimiterIndex + 1)..];

        return new SplitTokens
        {
            First = first.ToString(),
            Second = second.ToString()
        };
    }
}