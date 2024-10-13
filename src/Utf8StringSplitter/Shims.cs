
using System.Runtime.CompilerServices;
using System.Text;

#if NETSTANDARD2_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Utf8StringSplitter
{
    internal static class ArrayEx
    {
#if NET6_0_OR_GREATER
        public static readonly int MaxLength = Array.MaxLength;
#else
        public static readonly int MaxLength = 0x7fffffc7;
#endif
    }

    internal static class RuntimeHelpersEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceOrContainsReferences<T>()
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
            return true;
#endif
        }
    }

    internal static class UTF8Ex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty) return string.Empty;

#if NET5_0_OR_GREATER
            return Encoding.UTF8.GetString(bytes);
#else
            unsafe
            {
                fixed (byte* ptr = &MemoryMarshal.GetReference(bytes))
                {
                    return Encoding.UTF8.GetString(ptr, bytes.Length);
                }
            }
#endif
        }
    }
}
