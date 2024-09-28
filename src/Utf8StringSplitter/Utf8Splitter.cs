
namespace Utf8StringSplitter;

public static class Utf8Splitter
{
    public static SplitEnumerator Split(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter)
    {
        return new SplitEnumerator(target, delimiter);
    }

    public static SplitAnyEnumerator SplitAny(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter)
    {
        return new SplitAnyEnumerator(target, delimiter);
    }
}

public ref struct SplitEnumerator
{
    private ReadOnlySpan<byte> targetSpan;
    private readonly ReadOnlySpan<byte> delimiterSpan;

    public SplitEnumerator(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter)
    {
        targetSpan = target;
        delimiterSpan = delimiter;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var target = targetSpan;
        if (target.Length == 0) return false;

        var index = target.IndexOf(delimiterSpan);
        if (index < 0) // end
        {
            Current = target;
            targetSpan = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = target[..index];
        targetSpan = target[++index..];

        return true;
    }

}

public ref struct SplitAnyEnumerator
{
    private ReadOnlySpan<byte> targetSpan;
    private readonly ReadOnlySpan<byte> delimiterSpan;

    public SplitAnyEnumerator(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter)
    {
        targetSpan = target;
        delimiterSpan = delimiter;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitAnyEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var target = targetSpan;
        if (target.Length == 0) return false;

        var index = -1;
        foreach (var d in delimiterSpan)
        {
            var searchIndex = target.IndexOf(d);
            if ((uint)searchIndex < (uint)index)
            {
                index = searchIndex;
            }
        }

        if (index >= 0)
        {
            Current = target[..index];
            targetSpan = target[++index..];
            return true;
        }

        Current = target;
        targetSpan = ReadOnlySpan<byte>.Empty;
        return true;
    }

}