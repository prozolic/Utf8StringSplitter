
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Utf8StringSplitter;

public static class Utf8Splitter
{
    public static SplitEnumerator Split(ReadOnlySpan<byte> source, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((splitOptions & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        return new SplitEnumerator(source, delimiter, splitOptions);
    }

    public static SplitAnyEnumerator SplitAny(ReadOnlySpan<byte> source, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None, Utf8StringDelimiterOptions delimiterOptions = Utf8StringDelimiterOptions.MultiByte)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((splitOptions & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        switch(delimiterOptions)
        {
            case Utf8StringDelimiterOptions.MultiByte:
            case Utf8StringDelimiterOptions.SingleByte:
                return new SplitAnyEnumerator(source, delimiter, splitOptions, delimiterOptions);
            default:
                throw new ArgumentException("Utf8StringDelimiterOptions Value is Invalid.");
        }
    }
}

[Flags]
public enum Utf8StringSplitOptions
{
    None = 0,
    RemoveEmptyEntries = 1 << 0,
    TrimEntries = 1 << 1,
}

public enum Utf8StringDelimiterOptions
{
    MultiByte = 0,
    SingleByte = 1,
}

public ref struct SplitEnumerator
{
    private ReadOnlySpan<byte> source;
    private readonly ReadOnlySpan<byte> delimiter;
    private readonly Utf8StringSplitOptions options;
    private bool targetEmpty;

    public SplitEnumerator(ReadOnlySpan<byte> source, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions options)
    {
        this.source = source;
        this.delimiter = delimiter;
        this.options = options;
        targetEmpty = source.Length == 0;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (options != Utf8StringSplitOptions.None)
        {
            return MoveNextWithOptions();
        }

        var target = this.source;
        if (target.Length == 0)
        {
            if (targetEmpty)
            {
                targetEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        if (delimiter.Length == 0)
        {
            Current = target;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var indexForNone = target.IndexOf(delimiter);
        if (indexForNone < 0) // end
        {
            Current = target;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = target[..indexForNone];
        indexForNone += delimiter.Length;
        this.source = target[indexForNone..];
        targetEmpty = this.source.Length == 0;

        return true;
    }

    private bool MoveNextWithOptions()
    {
        var source = this.source;
        var removeEmptyEntries = (options & Utf8StringSplitOptions.RemoveEmptyEntries) == Utf8StringSplitOptions.RemoveEmptyEntries;
        var trimEntries = (options & Utf8StringSplitOptions.TrimEntries) == Utf8StringSplitOptions.TrimEntries;
        var (startIndex, endIndex) = (0, source.Length);

        if (source.Length == 0)
        {
            if (targetEmpty && !removeEmptyEntries)
            {
                targetEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        var index = source.IndexOf(delimiter);
        if (index < 0 || delimiter.Length == 0) // end
        {
            (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(source, startIndex, endIndex);

            if (trimEntries)
            {
                if (source.Length == 1 && source[0] == 0x20)
                {
                    source = ReadOnlySpan<byte>.Empty;
                }
                else
                {
                    source = source[startIndex..endIndex];
                }
            }
            if (removeEmptyEntries && source.Length == 0)
            {
                Current = default;
                return false;
            }
            Current = source;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var current = source[..index];
        index += delimiter.Length;
        source = source[index..];
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

        while (removeEmptyEntries && current.Length == 0)
        {
            index = source.IndexOf(delimiter);
            if (index < 0) // end
            {
                (startIndex, endIndex) = Utf8StringUtility.TrimSplitEntries(source, 0, source.Length);

                if (trimEntries)
                {
                    if (source.Length == 1 && source[0] == 0x20)
                    {
                        source = ReadOnlySpan<byte>.Empty;
                    }
                    else
                    {
                        source = source[startIndex..endIndex];
                    }
                }
                if (source.Length == 0)
                {
                    Current = default;
                    return false;
                }
                Current = source;
                this.source = ReadOnlySpan<byte>.Empty;
                return true;
            }

            current = source[..index];
            index += delimiter.Length;
            source = source[index..];
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
        this.source = source;
        targetEmpty = this.source.Length == 0;
        return true;
    }

}

public ref struct SplitAnyEnumerator
{
    private ReadOnlySpan<byte> source;
    private readonly ReadOnlySpan<byte> delimiter;
    private readonly Utf8StringSplitOptions options;
    private readonly Utf8StringDelimiterOptions delimiterOptions;
    private bool targetEmpty;

    public SplitAnyEnumerator(ReadOnlySpan<byte> source, ReadOnlySpan<byte> delimiter, Utf8StringSplitOptions options, Utf8StringDelimiterOptions delimiterOptions)
    {
        this.source = source;
        this.delimiter = delimiter;
        this.options = options;
        this.delimiterOptions = delimiterOptions;
        targetEmpty = source.Length == 0;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitAnyEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (options != Utf8StringSplitOptions.None)
        {
            return MoveNextWithOptions();
        }

        var source = this.source;
        if (source.Length == 0)
        {
            if (targetEmpty)
            {
                targetEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        if (delimiter.Length == 0)
        {
            Current = source;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var indexForNone = -1;
        var sequenceLength = 1;
        foreach (var d in new DelimiterEnumerator(delimiter, delimiterOptions))
        {
            var searchIndex = source.IndexOf(d);
            if ((uint)searchIndex < (uint)indexForNone)
            {
                indexForNone = searchIndex;
                sequenceLength = d.Length;
            }
        }
        if (indexForNone < 0) // end
        {
            Current = source;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = source[..indexForNone];
        indexForNone += sequenceLength;
        this.source = source[indexForNone..];
        targetEmpty = this.source.Length == 0;

        return true;
    }

    private bool MoveNextWithOptions()
    {
        var target = this.source;
        var removeEmptyEntries = (options & Utf8StringSplitOptions.RemoveEmptyEntries) == Utf8StringSplitOptions.RemoveEmptyEntries;
        var trimEntries = (options & Utf8StringSplitOptions.TrimEntries) == Utf8StringSplitOptions.TrimEntries;
        var (startIndex, endIndex) = (0, target.Length);

        if (target.Length == 0)
        {
            if (targetEmpty && !removeEmptyEntries)
            {
                targetEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        var index = -1;
        var sequenceLength = 1;
        foreach (var d in new DelimiterEnumerator(delimiter, delimiterOptions))
        {
            var searchIndex = target.IndexOf(d);
            if ((uint)searchIndex < (uint)index)
            {
                index = searchIndex;
                sequenceLength = d.Length;
            }
        }
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
            if (removeEmptyEntries && target.Length == 0)
            {
                Current = default;
                return false;
            }
            Current = target;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var current = target[..index];
        index += sequenceLength;
        target = target[index..];
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

        while (removeEmptyEntries && current.Length == 0)
        {
            index = -1;
            sequenceLength = 1;
            foreach (var d in new DelimiterEnumerator(delimiter, delimiterOptions))
            {
                var searchIndex = target.IndexOf(d);
                if ((uint)searchIndex < (uint)index)
                {
                    index = searchIndex;
                    sequenceLength = d.Length;
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
                if (target.Length == 0)
                {
                    Current = default;
                    return false;
                }
                Current = target;
                this.source = ReadOnlySpan<byte>.Empty;
                return true;
            }

            current = target[..index];
            index += sequenceLength;
            target = target[index..];
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
        this.source = target;
        targetEmpty = this.source.Length == 0;
        return true;
    }

}

internal ref struct DelimiterEnumerator
{
    private ReadOnlySpan<byte> delimiter;
    private Utf8StringDelimiterOptions delimiterOptions;

    public DelimiterEnumerator(ReadOnlySpan<byte> delimiter, Utf8StringDelimiterOptions delimiterOptions)
    {
        this.delimiter = delimiter;
        this.delimiterOptions = delimiterOptions;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly DelimiterEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var delimiter = this.delimiter;
        if (delimiter.Length == 0)
        {
            return false;
        }

        if (this.delimiter.Length == 0)
        {
            Current = delimiter;
            this.delimiter = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var sequenceLength = Utf8StringUtility.GetUtf8SequenceLength(delimiter, delimiterOptions);
        if (sequenceLength < 0) // end
        {
            Current = delimiter;
            this.delimiter = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = delimiter[..sequenceLength];
        this.delimiter = delimiter[sequenceLength..];

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

    public static int GetUtf8SequenceLength(ReadOnlySpan<byte> bytes, Utf8StringDelimiterOptions delimiterOptions)
    {
        if (delimiterOptions == Utf8StringDelimiterOptions.SingleByte)
            return 1;

        ref var refBytes = ref MemoryMarshal.GetReference(bytes);
        var i = 0;
        ref var b1 = ref Unsafe.Add(ref refBytes, i);
        if ((b1 & 0x80) == 0x00) return 1;
        else if (((b1 & 0xe0) == 0xc0)) goto Two;
        else if (((b1 & 0xf0) == 0xe0)) goto Three;
        else if (((b1 & 0xf8) == 0xf0)) goto Four;
        else return -1;

    Two:
        if (bytes.Length <= ++i) return -1;
        if ((Unsafe.Add(ref refBytes, i) & 0xc0) != 0x80) return -1;
        return 2;

    Three:
        if (bytes.Length <= i + 2) return -1;

        if (Unsafe.Add(ref refBytes, i) == 0xe0)
        {
            ref var b2 = ref Unsafe.Add(ref refBytes, i + 1);
            if (0x7f < b2 && b2 < 0xa0) return -1;
        }
        else if (Unsafe.Add(ref refBytes, i) == 0xed) // surrogate pair
        {
            if (0x9f < Unsafe.Add(ref refBytes, i + 1)) return -1;
        }
        if ((Unsafe.Add(ref refBytes, ++i) & 0xc0) != 0x80) return -1;
        if ((Unsafe.Add(ref refBytes, ++i) & 0xc0) != 0x80) return -1;
        return 3;

    Four:
        if (bytes.Length <= i + 3) return -1;

        if (Unsafe.Add(ref refBytes, i) == 0xf0)
        {
            ref var b2 = ref Unsafe.Add(ref refBytes, i + 1);
            if (0x7f < b2 && b2 < 0x90) return -1;
        }
        if ((Unsafe.Add(ref refBytes, ++i) & 0xc0) != 0x80) return -1;
        if ((Unsafe.Add(ref refBytes, ++i) & 0xc0) != 0x80) return -1;
        if ((Unsafe.Add(ref refBytes, ++i) & 0xc0) != 0x80) return -1;
        return 4;
    }

}
