using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Utf8StringSplitter;


foreach (var s in Utf8Splitter.Split("     ,2, 3,4,5"u8, ","u8, Utf8StringSplitOptions.None))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}


foreach (var s in Utf8Splitter.Split("   , 2  , [3] , 4, 5 6 "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}


foreach (var s in Utf8Splitter.Split(",,    ,4,   5, 6 7 ,,   "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.Split("  ,2 ,  ,, 5  6 "u8, ","u8, Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.SplitAny("1;2-3,4-5"u8, ",-;"u8))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.SplitAny("1; 2- 3 ,4- 5,,,6,"u8, ",-;"u8, Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.SplitAny("あいうえうお"u8, "う"u8))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.SplitAny("あいうえうお"u8, "う"u8, delimiterOptions:Utf8StringDelimiterOptions.SingleByte))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in "".Split(""))
{
    Console.WriteLine($"[{s}]");
}

foreach (var s in "   , 2  , [3] , 4,5  ".Split(",", StringSplitOptions.TrimEntries))
{
    Console.WriteLine($"[{s}]");
}


foreach (var s in ",,    ,4,   5, 6 ,,  , , ,  , , , , , , , , , , , ,,  , ".Split(",", StringSplitOptions.RemoveEmptyEntries))
{
    Console.WriteLine($"[{s}]");
}


foreach (var s in "  ,2 ,  ,,5   ".Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
{
    Console.WriteLine($"[{s}]");
}


Console.WriteLine("--------------------");

ReadOnlySpan<char> source = "あいうえお".AsSpan();
Span<Range> destination = stackalloc Range[100];
ReadOnlySpan<char> separators = "う".AsSpan();

int count = MemoryExtensions.SplitAny(source, destination, separators, StringSplitOptions.None);

for (int i = 0; i < count; i++)
{
    Console.WriteLine($"{source[destination[i]]}");
}

Console.WriteLine("End");
