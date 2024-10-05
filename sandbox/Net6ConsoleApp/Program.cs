using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Utf8StringSplitter;

void Sample()
{
    // u8 suffix is a C# 11 feature
    ReadOnlySpan<byte> utf8Source = "1,2,3,4,5"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source, (byte)','))
    {
        // ToArray for .NET Standard 2.0
        //Console.WriteLine($"{Encoding.UTF8.GetString(str.ToArray())}");
        Console.WriteLine($"{Encoding.UTF8.GetString(str.ToArray())}");
    }
    Console.WriteLine("--------------------");

    // u8 suffix is a C# 11 feature
    ReadOnlySpan<byte> utf8Source2 = "1--2--3--4--5"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source2, "--"u8))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }
    Console.WriteLine("--------------------");

    // u8 suffix is a C# 11 feature
    ReadOnlySpan<byte> utf8Source3 = "1,2-3;4-5"u8;
    foreach (var str in Utf8Splitter.SplitAny(utf8Source3, "-,;"u8))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }
}

void SampleSplit()
{
    // default
    Console.WriteLine("Utf8Splitter.Split");
    ReadOnlySpan<byte> utf8Source = "1,2,3,4,5"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source, (byte)','))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    Console.WriteLine("Utf8Splitter.Split");
    ReadOnlySpan<byte> utf8Source2 = "1---2---3---4---5"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source2, "---"u8))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    // splitOptions is TrimEntries.
    Console.WriteLine("Utf8Splitter.Split : Utf8StringSplitOptions.TrimEntries");
    ReadOnlySpan<byte> utf8Source3 = " 1 , 2 , 3 , 4 , 5 "u8;
    foreach (var str in Utf8Splitter.Split(utf8Source3, (byte)',', splitOptions: Utf8StringSplitOptions.TrimEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    // splitOptions is RemoveEmptyEntries.
    Console.WriteLine("Utf8Splitter.Split : Utf8StringSplitOptions.RemoveEmptyEntries");
    ReadOnlySpan<byte> utf8Source4 = ",1,2,,,,3,,4,,5,,"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source4, (byte)',', splitOptions: Utf8StringSplitOptions.RemoveEmptyEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    // splitOptions is TrimEntries and RemoveEmptyEntries.
    Console.WriteLine("Utf8Splitter.Split : Utf8StringSplitOptions.TrimEntries and RemoveEmptyEntries");
    ReadOnlySpan<byte> utf8Source5 = " ,1,  2, ,,  ,  3 ,,4,, 5 ,,"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source5, (byte)',', 
        splitOptions: Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }
}

void SampleSplitAny()
{
    Console.WriteLine("Utf8Splitter.SplitAny");
    foreach (var s in Utf8Splitter.SplitAny("1;2-3,4-5"u8, ",-;"u8))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(s)}");
    }

    Console.WriteLine("Utf8Splitter.SplitAny");
    foreach (var s in Utf8Splitter.SplitAny("1😀2🙃3😋4😀5"u8, "😀🙃😋"u8))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(s)}");
    }

    // splitOptions is TrimEntries.
    Console.WriteLine("Utf8Splitter.SplitAny : Utf8StringSplitOptions.TrimEntries");
    ReadOnlySpan<byte> utf8Source3 = " 1 , 2 - 3 ; 4 , 5 "u8;
    foreach (var str in Utf8Splitter.Split(utf8Source3, ",-;"u8, splitOptions: Utf8StringSplitOptions.TrimEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    // splitOptions is RemoveEmptyEntries.
    Console.WriteLine("Utf8Splitter.SplitAny : Utf8StringSplitOptions.RemoveEmptyEntries");
    ReadOnlySpan<byte> utf8Source4 = ",1,2,--,3,,4;,5;,"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source4, ",-;"u8, splitOptions: Utf8StringSplitOptions.RemoveEmptyEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }

    // splitOptions is TrimEntries and RemoveEmptyEntries.
    Console.WriteLine("Utf8Splitter.SplitAny : Utf8StringSplitOptions.TrimEntries and RemoveEmptyEntries");
    ReadOnlySpan<byte> utf8Source5 = " ,1-  2, ,-  ,  3 ,,4,; 5 ,-"u8;
    foreach (var str in Utf8Splitter.Split(utf8Source5, ",-;"u8,
        splitOptions: Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
    {
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
    }
}

void SampleSplitAny2()
{
    Console.WriteLine("Utf8Splitter.SplitAny Utf8StringSeparatorOptions.Utf8");
    foreach (var s in Utf8Splitter.SplitAny("1😀2🙃3😋4😀5"u8, "😀🙃😋"u8, separatorOptions: Utf8StringSeparatorOptions.Utf8))
    {
        for (var i = 0;i < s.Length; i++)
        {
            Console.Write($"{s[i]}");
        }
        Console.WriteLine();
    }

    Console.WriteLine("Utf8Splitter.SplitAny Utf8StringSeparatorOptions.Bytes");
    foreach (var s in Utf8Splitter.SplitAny("1😀2🙃3😋4😀5"u8, "😀🙃😋"u8, separatorOptions:Utf8StringSeparatorOptions.Bytes))
    {
        for (var i = 0; i < s.Length; i++)
        {
            Console.Write($"{s[i]}");
        }
        Console.WriteLine();
    }
}

Sample();
SampleSplit();
SampleSplitAny();
SampleSplitAny2();

foreach (var s in Utf8Splitter.Split("     ,2, 3,4,5"u8, ","u8, Utf8StringSplitOptions.None))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.Split("     ,2, 3,4,5"u8, (byte)',', Utf8StringSplitOptions.None))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.Split("1---2---3---4---5"u8, "---"u8, Utf8StringSplitOptions.None))
{
    Console.WriteLine($"[{Encoding.UTF8.GetString(s)}]");
}

foreach (var s in Utf8Splitter.Split("あいうえうお"u8, "う"u8))
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

foreach (var s in Utf8Splitter.SplitAny("あいうえうお"u8, "う"u8, separatorOptions:Utf8StringSeparatorOptions.Bytes))
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
