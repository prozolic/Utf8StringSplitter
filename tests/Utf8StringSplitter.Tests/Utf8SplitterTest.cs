using FluentAssertions;
using System.Text;
using Utf8StringSplitter;

namespace Utf8StringSplitter.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void SplitTest()
        {
            var index = 0;
            var bytes = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }; // 1, 2, 3, 4, 5
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(bytes[index++]);
            }

            index.Should().Be(5);
        }

        [Fact]
        public void SplitAnyTest()
        {
            var index = 0;
            var bytes = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }; // 1, 2, 3, 4, 5
            foreach (var s in Utf8Splitter.SplitAny("1,2-3;4-5"u8, "-,;"u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(bytes[index++]);
            }

            index.Should().Be(5);
        }
    }
}