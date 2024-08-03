using System.Text;

namespace DataProcessor.Domain;

/**
 * Source: https://hotforknowledge.com/2024/01/13/1brc-in-dotnet-among-fastest-on-linux-my-optimization-journey/#fast-utf8spanequals
 */
public readonly unsafe struct ByteSpanEquatable(byte* pointer, int length) : IEquatable<ByteSpanEquatable>
{
    public ReadOnlySpan<byte> Span => new(pointer, length);

    public bool Equals(ByteSpanEquatable other) => Span.SequenceEqual(other.Span);

    // It was that lazy! Did not even used freely available additional entropy from _len in the hash. 
    // But it worked quite well with the default dataset.
    public override int GetHashCode()
    {
        return length switch
        {
            // Use first bytes as the hash
            > 3 => *(int*)pointer,
            > 1 => *(short*)pointer,
            > 0 => *pointer,
            _ => 0
        };
    }

    public override bool Equals(object? obj)
    {
        return obj is ByteSpanEquatable other && Equals(other);
    }

    public override string ToString()
    {
        return new string((sbyte*)pointer, 0, length, Encoding.UTF8);
    }
}