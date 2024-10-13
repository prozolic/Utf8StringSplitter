
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Utf8StringSplitter;

public static class Utf8Splitter
{
    public static SplitEnumerator Split(ReadOnlySpan<byte> source, byte separator, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((splitOptions & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        return new SplitEnumerator(source, separator, splitOptions);
    }

    public static SplitEnumerator Split(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separator, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((splitOptions & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        return new SplitEnumerator(source, separator, splitOptions);
    }

    public static SplitAnyEnumerator SplitAny(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separators, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None, Utf8StringSeparatorOptions separatorOptions = Utf8StringSeparatorOptions.Utf8)
    {
        const Utf8StringSplitOptions AllOptions = Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries;
        if ((splitOptions & ~AllOptions) != 0)
        {
            throw new ArgumentException("Utf8StringSplitOptions Value is Invalid.");
        }

        switch(separatorOptions)
        {
            case Utf8StringSeparatorOptions.Utf8:
            case Utf8StringSeparatorOptions.Bytes:
                return new SplitAnyEnumerator(source, separators, splitOptions, separatorOptions);
            default:
                throw new ArgumentException("Utf8StringSeparatorOptions Value is Invalid.");
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

public enum Utf8StringSeparatorOptions
{
    Utf8 = 0,
    Bytes = 1,
}

public ref struct SplitEnumerator
{
    private ReadOnlySpan<byte> source;
    private readonly byte separatorByte;
    private readonly ReadOnlySpan<byte> separatorBytes;
    private readonly Utf8StringSplitOptions options;
    private bool sourceEmpty;
    private bool singleSeparator;

    internal SplitEnumerator(ReadOnlySpan<byte> source, byte separator, Utf8StringSplitOptions options)
    {
        this.source = source;
        sourceEmpty = source.Length == 0;
        singleSeparator = true;

        this.separatorByte = separator;
        this.separatorBytes = ReadOnlySpan<byte>.Empty;
        this.options = options;
    }

    internal SplitEnumerator(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separator, Utf8StringSplitOptions options)
    {
        this.source = source;
        sourceEmpty = source.Length == 0;
        singleSeparator = separator.Length == 1;

        if (singleSeparator)
        {
            this.separatorByte = separator[0];
            this.separatorBytes = ReadOnlySpan<byte>.Empty;
            this.options = options;
        }
        else
        {
            this.separatorBytes = separator;
            this.options = options;
        }
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitEnumerator GetEnumerator() => this;

    internal readonly int sourceLength => source.Length;

    public bool MoveNext()
    {
        if (singleSeparator)
        {
            Span<byte> separator = stackalloc byte[1];
            separator[0] = this.separatorByte;

            return MoveNextInternal(separator);
        }

        return MoveNextInternal(this.separatorBytes);
    }

    public readonly byte[][] ToArray()
    {
        var writer = new ExtendableArray<byte[]>(sourceLength);
        foreach (var i in this)
        {
            writer.Add(i.ToArray());
        }

        return writer.AsSpan().ToArray();
    }

    public readonly string[] ToUtf16Array()
    {
        var writer = new ExtendableArray<string>(sourceLength);
        foreach (var i in this)
        {
            writer.Add(UTF8Ex.GetString(i));
        }

        return writer.AsSpan().ToArray();
    }

    private bool MoveNextInternal(scoped ReadOnlySpan<byte> separator)
    {
        if (options != Utf8StringSplitOptions.None)
        {
            return MoveNextInternalWithOptions(separator);
        }

        var target = this.source;
        if (target.Length == 0)
        {
            if (sourceEmpty)
            {
                sourceEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        if (separator.Length == 0)
        {
            Current = target;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var indexForNone = target.IndexOf(separator);
        if (indexForNone < 0) // end
        {
            Current = target;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = target[..indexForNone];
        indexForNone += separator.Length;
        this.source = target[indexForNone..];
        sourceEmpty = this.source.Length == 0;

        return true;
    }

    private bool MoveNextInternalWithOptions(scoped ReadOnlySpan<byte> separator)
    {
        var source = this.source;
        var removeEmptyEntries = (options & Utf8StringSplitOptions.RemoveEmptyEntries) == Utf8StringSplitOptions.RemoveEmptyEntries;
        var trimEntries = (options & Utf8StringSplitOptions.TrimEntries) == Utf8StringSplitOptions.TrimEntries;
        var (startIndex, endIndex) = (0, source.Length);

        if (source.Length == 0)
        {
            if (sourceEmpty && !removeEmptyEntries)
            {
                sourceEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        var index = source.IndexOf(separator);
        if (index < 0 || separator.Length == 0) // end
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
        index += separator.Length;
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
            index = source.IndexOf(separator);
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
            index += separator.Length;
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
        sourceEmpty = this.source.Length == 0;
        return true;
    }

}

public ref struct SplitAnyEnumerator
{
    private ReadOnlySpan<byte> source;
    private readonly ReadOnlySpan<byte> separators;
    private readonly Utf8StringSplitOptions options;
    private readonly Utf8StringSeparatorOptions separatorOptions;
    private bool sourceEmpty;

    internal SplitAnyEnumerator(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separators, Utf8StringSplitOptions options, Utf8StringSeparatorOptions separatorOptions)
    {
        this.source = source;
        this.separators = separators;
        this.options = options;
        this.separatorOptions = separatorOptions;
        sourceEmpty = source.Length == 0;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SplitAnyEnumerator GetEnumerator() => this;

    internal readonly int sourceLength => source.Length;

    public bool MoveNext()
    {
        if (options != Utf8StringSplitOptions.None)
        {
            return MoveNextWithOptions();
        }

        var source = this.source;
        if (source.Length == 0)
        {
            if (sourceEmpty)
            {
                sourceEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        if (separators.Length == 0)
        {
            Current = source;
            this.source = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var indexForNone = -1;
        var sequenceLength = 1;
        foreach (var d in new SeparatorEnumerator(separators, separatorOptions))
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
        sourceEmpty = this.source.Length == 0;

        return true;
    }

    public readonly byte[][] ToArray()
    {
        var writer = new ExtendableArray<byte[]>(sourceLength);
        foreach (var i in this)
        {
            writer.Add(i.ToArray());
        }

        return writer.AsSpan().ToArray();
    }

    public readonly string[] ToUtf16Array()
    {
        var writer = new ExtendableArray<string>(sourceLength);
        foreach (var i in this)
        {
            writer.Add(UTF8Ex.GetString(i));
        }

        return writer.AsSpan().ToArray();
    }

    private bool MoveNextWithOptions()
    {
        var target = this.source;
        var removeEmptyEntries = (options & Utf8StringSplitOptions.RemoveEmptyEntries) == Utf8StringSplitOptions.RemoveEmptyEntries;
        var trimEntries = (options & Utf8StringSplitOptions.TrimEntries) == Utf8StringSplitOptions.TrimEntries;
        var (startIndex, endIndex) = (0, target.Length);

        if (target.Length == 0)
        {
            if (sourceEmpty && !removeEmptyEntries)
            {
                sourceEmpty = false;
                Current = ReadOnlySpan<byte>.Empty;
                return true;
            }

            return false;
        }

        var index = -1;
        var sequenceLength = 1;
        foreach (var d in new SeparatorEnumerator(separators, separatorOptions))
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
            foreach (var d in new SeparatorEnumerator(separators, separatorOptions))
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
        sourceEmpty = this.source.Length == 0;
        return true;
    }

}

internal ref struct SeparatorEnumerator
{
    private ReadOnlySpan<byte> separator;
    private Utf8StringSeparatorOptions separatorOptions;

    public SeparatorEnumerator(ReadOnlySpan<byte> separator, Utf8StringSeparatorOptions separatorOptions)
    {
        this.separator = separator;
        this.separatorOptions = separatorOptions;
    }

    public ReadOnlySpan<byte> Current { get; private set; } = default;

    public readonly SeparatorEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var separator = this.separator;
        if (separator.Length == 0)
        {
            return false;
        }

        if (this.separator.Length == 0)
        {
            Current = separator;
            this.separator = ReadOnlySpan<byte>.Empty;
            return true;
        }

        var sequenceLength = Utf8StringUtility.GetUtf8SequenceLength(separator, separatorOptions);
        if (sequenceLength < 0) // end
        {
            Current = separator;
            this.separator = ReadOnlySpan<byte>.Empty;
            return true;
        }

        Current = separator[..sequenceLength];
        this.separator = separator[sequenceLength..];

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

    public static int GetUtf8SequenceLength(ReadOnlySpan<byte> bytes, Utf8StringSeparatorOptions separatorOptions)
    {
        if (separatorOptions == Utf8StringSeparatorOptions.Bytes)
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

internal struct ExtendableArray<T>
{
    private T[] array;
    private int count;
    private bool isRent;

    public readonly int Count => count;

    public readonly bool IsRent => isRent;

    public ExtendableArray(int capacity)
    {
        array = ArrayPool<T>.Shared.Rent(capacity);
        count = 0;
        isRent = true;
    }

    public readonly ReadOnlySpan<T> AsSpan()
        => array.AsSpan(0, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        var count = this.count;
        var array = this.array;
        if ((uint)count < (uint)array.Length)
        {
            array[count++] = value;
            this.count = count;
        }
        else
        {
            this.AddAndEnsureCapacity(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return()
    {
        count = 0;
        ArrayPool<T>.Shared.Return(array, RuntimeHelpersEx.IsReferenceOrContainsReferences<T>());
        array = [];
        isRent = false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddAndEnsureCapacity(T value)
    {
        var count = this.count;
        var oldArray = array;

        var newArray = ArrayPool<T>.Shared.Rent(Math.Min(oldArray.Length * 2, ArrayEx.MaxLength));
        Array.Copy(oldArray, newArray, count);
        ArrayPool<T>.Shared.Return(oldArray, RuntimeHelpersEx.IsReferenceOrContainsReferences<T>());

        newArray[count] = value;
        this.count = count + 1;
        this.array = newArray;
    }

}
