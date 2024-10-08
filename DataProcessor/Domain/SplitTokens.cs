namespace DataProcessor.Domain;

public struct SplitTokens
{
    public string First;
    public string Second;
}

public struct SplitBytesTokens
{
    public byte[] First;
    public byte[] Second;
}

public struct SplitCharsTokens
{
    public char[] First;
    public char[] Second;
}

public ref struct UnsafeSplitBytesTokens
{
    public ReadOnlySpan<byte> First;
    public ReadOnlySpan<byte> Second;
}