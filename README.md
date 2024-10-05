Utf8StringSplitter
===

`Utf8StringSplitter` provides methods (`Split` and `SplitAny`) to split Utf8 strings (`byte` sequences) based on a specified separators.  
This library is distributed via NuGet, supporting .NET Standard 2.0, .NET Standard 2.1, .NET 6 (.NET 7), .NET 8 and above.

> PM> Install-Package [Utf8StringSplitter](https://www.nuget.org/packages/Utf8StringSplitter/)

How to use
---

```csharp

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
        Console.WriteLine($"{Encoding.UTF8.GetString(str)}");
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
```

```csharp
// output
// 1
// 2
// 3
// 4
// 5
// --------------------
// 1
// 2
// 3
// 4
// 5
// --------------------
// 1
// 2
// 3
// 4
// 5
```

`Utf8Splitter` API
---

```csharp
public static class Utf8Splitter
{
    public static SplitEnumerator Split(ReadOnlySpan<byte> source, byte separator, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None);
    public static SplitEnumerator Split(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separator, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None);
    public static SplitAnyEnumerator SplitAny(ReadOnlySpan<byte> source, ReadOnlySpan<byte> separators, Utf8StringSplitOptions splitOptions = Utf8StringSplitOptions.None, Utf8StringSeparatorOptions separatorOptions = Utf8StringSeparatorOptions.MultiByte);
}
```

`Utf8StringSplitter.Split`
---

`Utf8StringSplitter.Split` Split and enumerate a UTF8 string into `ReadOnlySpan<byte>` based on the separators.
The separators can specify `byte` or `ReadOnlySpan<byte>`.
Option can specify `Utf8StringSplitOptions` almost equivalent to [`StringSplitOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.stringsplitoptions?view=net-8.0).

```csharp
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

```

```csharp
// output
// Utf8Splitter.Split
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.Split
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.Split : Utf8StringSplitOptions.TrimEntries
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.Split : Utf8StringSplitOptions.RemoveEmptyEntries
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.Split : Utf8StringSplitOptions.TrimEntries and RemoveEmptyEntries
// 1
// 2
// 3
// 4
// 5
```

`Utf8StringSplitter.SplitAny`
---

`Utf8StringSplitter.SplitAny` Split and enumerate a UTF8 string into `ReadOnlySpan<byte>` for one of the specified separators.  
The first option can specify `Utf8StringSplitOptions` almost equivalent to [`StringSplitOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.stringsplitoptions?view=net-8.0).

```csharp
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
```

```csharp
// output
// Utf8Splitter.SplitAny
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.SplitAny
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.SplitAny : Utf8StringSplitOptions.TrimEntries
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.SplitAny : Utf8StringSplitOptions.RemoveEmptyEntries
// 1
// 2
// 3
// 4
// 5
// Utf8Splitter.SplitAny : Utf8StringSplitOptions.TrimEntries and RemoveEmptyEntries
// 1
// 2
// 3
// 4
// 5
```
  
The second option can specify `Utf8StringSeparatorOptions`.  
`Utf8StringSeparatorOptions.Utf8` processes separators as UTF-8 string one by one. `Utf8StringSeparatorOptions.Bytes` processes separators as byte array.
The default value is `Utf8StringSeparatorOptions.Utf8`.

```csharp
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
```

```csharp
// output
// Utf8Splitter.SplitAny Utf8StringSeparatorOptions.Utf8
// 49
// 50
// 51
// 52
// 53
// Utf8Splitter.SplitAny Utf8StringSeparatorOptions.Bytes
// 49
// 
// 
// 
// 50
// 
// 
// 
// 51
// 
// 
// 
// 52
// 
// 
// 
// 53
```

License
---

MIT License.
