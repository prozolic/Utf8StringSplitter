
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Utf8StringSplitter;

public static class Utf8Splitter
{
    public static SplitEnumerator Split(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions options = Utf8StringSplitOptions.None)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((options & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        return new SplitEnumerator(target, delimiter, options);
    }

    public static SplitEnumerator SplitAny(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions options = Utf8StringSplitOptions.None)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((options & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        return new SplitEnumerator(target, delimiter, options);
    }
}

[Flags]
public enum Utf8StringSplitOptions
{
    None = 0,
    RemoveEmptyEntries = 1 << 0,
    TrimEntries = 1 << 1,
}

public ref struct SplitEnumerator
{
    private ReadOnlySpan<byte> target;
    private readonly ReadOnlySpan<byte> delimiter;
    private readonly Utf8StringSplitOptions options;

    public SplitEnumerator(ReadOnlySpan<byte> target, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions options)
    {
        this.target = target;
        this.delimiter = delimiter;
        this.options = options;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var target = this.target;
        if (target.Length == 0) return false;

        if (options != Utf8StringSplitOptions.None)
        {
            return MoveNextWithOptions();
        }

        var indexForNone = -1;
        foreach (var d in delimiter)
        {
            var searchIndex = target.IndexOf(d);
            if ((uint)searchIndex < (uint)indexForNone)
            {
                indexForNone = searchIndex;
            }
        }
        if (indexForNone < 0) // end
        {
            Current = target;
            this.target = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = target[..indexForNone];
        this.target = target[++indexForNone..];

        return true;
    }

    private bool MoveNextWithOptions()
    {
        var target = this.target;
        var removeEmptyEntries = (options & Utf8StringSplitOptions.RemoveEmptyEntries) == Utf8StringSplitOptions.RemoveEmptyEntries;
        var trimEntries = (options & Utf8StringSplitOptions.TrimEntries) == Utf8StringSplitOptions.TrimEntries;
        var (startIndex, endIndex) = (0, target.Length);
        var index = target.IndexOf(delimiter);

        if (index < 0) // end
        {
            (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(target, startIndex, endIndex);

            if (trimEntries)
            {
                if (target.Length == 1 && target[0] == 0x20)
                {
                    target = ReadOnlySpan<byte>.Empty;
                }
                else
                {
                    target = target[startIndex..endIndex];
                }
            }
            if (removeEmptyEntries && ((endIndex - startIndex) == 0))
            {
                Current = default;
                return false;
            }
            Current = target;
            this.target = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var current = target[..index];
        target = target[++index..];
        if (trimEntries)
        {
            (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(current, 0, current.Length);
            if (current.Length == 1 && current[0] == 0x20)
            {
                current = ReadOnlySpan<byte>.Empty;
            }
            else
            {
                current = current[startIndex..endIndex];
            }
        }
        else
        {
            (startIndex, endIndex) = (0, index - 1);
        }

        while (removeEmptyEntries && ((endIndex - startIndex) == 0))
        {
            index = -1;
            foreach (var d in delimiter)
            {
                var searchIndex = target.IndexOf(d);
                if ((uint)searchIndex < (uint)index)
                {
                    index = searchIndex;
                }
            }
            if (index < 0) // end
            {
                (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(target, 0, target.Length);

                if (trimEntries)
                {
                    if (target.Length == 1 && target[0] == 0x20)
                    {
                        target = ReadOnlySpan<byte>.Empty;
                    }
                    else
                    {
                        target = target[startIndex..endIndex];
                    }
                }
                if (removeEmptyEntries && ((endIndex - startIndex) == 0))
                {
                    Current = default;
                    return false;
                }
                Current = target;
                this.target = ReadOnlySpan<byte>.Empty;
                return true;
            }

            current = target[..index];
            target = target[++index..];
            if (trimEntries)
            {
                (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(current, 0, current.Length);
                if (current.Length == 1 && current[0] == 0x20)
                {
                    current = ReadOnlySpan<byte>.Empty;
                }
                else
                {
                    current = current[startIndex..endIndex];
                }
            }
            else
            {
                (startIndex, endIndex) = (0, index - 1);
            }
        }

        Current = current;
        this.target = target;
        return true;
    }

}

internal static class Utf8StringUtility
{
    public static (int startIndex, int endIndex) TrimSplitEntries(ReadOnlySpan<byte> target, int startIndex, int endIndex)
    {
        const byte Space = 0x20;
        ref var reftarget = ref MemoryMarshal.GetReference(target);

        while (startIndex < endIndex && Space == Unsafe.Add(ref reftarget, startIndex))
        {
            startIndex++;
        }

        while (startIndex < endIndex && Space == Unsafe.Add(ref reftarget, endIndex - 1))
        {
            endIndex--;
        }

        return (startIndex, endIndex);
    }

}