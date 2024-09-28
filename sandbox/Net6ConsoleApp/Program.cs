using System.Text;
using Utf8StringSplitter;

Console.WriteLine("Hello, World!");

foreach(var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8))
{
    Console.WriteLine(Encoding.UTF8.GetString(s));
}
Console.WriteLine("--------------------");
foreach (var s in Utf8Splitter.SplitAny("1;2-3,4-5"u8, ",-;"u8))
{
    Console.WriteLine(Encoding.UTF8.GetString(s));
}
Console.WriteLine("--------------------");

ReadOnlySpan<char> source = "apple,banana;cherry".AsSpan();
Span<Range> destination = stackalloc Range[3];
ReadOnlySpan<char> separators = ",;".AsSpan();

int count = MemoryExtensions.SplitAny(source, destination, separators, StringSplitOptions.None);

for (int i = 0; i < count; i++)
{
    Console.WriteLine($"{source[destination[i]]}");
}

Console.WriteLine("End");
