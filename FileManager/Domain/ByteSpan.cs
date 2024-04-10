namespace FileManager.Domain;

/// <summary>
/// Span used to represent an UTF8 string.
/// It extends from IEquatable such that it mutates the default Dictionary key comparison behaviour.
/// </summary>
public readonly unsafe struct ByteSpan: IEquatable<ByteSpan>
{
    private readonly char* ptr;
    private readonly int len;

    public ReadOnlySpan<char> Span()
    {
        return new ReadOnlySpan<char>(ptr, len);
    }
    
    /*public override int GetHashCode()
    {
        return len switch
        {
            // Use first bytes as the hash
            > 3 => *(int*)ptr,
            > 1 => *(short*)ptr,
            > 0 => *ptr,
            _ => 0
        };
    }*/

    public bool Equals(ByteSpan other)
    {
        return Span().SequenceEqual(other.Span());
    }

    /*public override bool Equals(object? obj)
    {
        return obj is ByteSpan other && Equals(other);
    }*/
    
}