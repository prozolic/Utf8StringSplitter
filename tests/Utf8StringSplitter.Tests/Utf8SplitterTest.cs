using FluentAssertions;
using System;
using System.Diagnostics;

namespace Utf8StringSplitter.Tests
{
    public class Utf8SplitterTest
    {
        [Fact]
        public void SplitTest()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitTest2()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitTest3()
        {
            var index = 0;
            var expected = new List<byte[]>()
            {
                ""u8.ToArray(),
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                ""u8.ToArray(),
                "3"u8.ToArray(),
                " "u8.ToArray(),
                "4"u8.ToArray(),
                "5"u8.ToArray(),
                ""u8.ToArray(),
            };
            foreach (var s in Utf8Splitter.Split("--1--2----3-- --4--5--"u8, "--"u8))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesTest()
        {
            var expected = new List<byte[]>()
            {
                ""u8.ToArray(),
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                "3"u8.ToArray(),
                ""u8.ToArray(),
                "4"u8.ToArray(),
                "5"u8.ToArray(),
                ""u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                ""u8.ToArray(),
                "1"u8.ToArray(),
                "2 3"u8.ToArray(),
                ""u8.ToArray(),
                ""u8.ToArray(),
                "45"u8.ToArray(),
                ""u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2 3 ,,    ,45,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                index++;
                s.ToArray().Should().Equal(""u8.ToArray());
            }

            index.Should().Be(1);
        }

        [Fact]
        public void SplitWithTrimEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesTest6()
        {
            var index = 0;
            var expected = new List<byte[]>()
            {
                ""u8.ToArray(),
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                ""u8.ToArray(),
                "3"u8.ToArray(),
                ""u8.ToArray(),
                "4"u8.ToArray(),
                "5"u8.ToArray(),
                ""u8.ToArray(),
            };
            foreach (var s in Utf8Splitter.Split("-- 1--2 ----3-- -- 4 --5--"u8, "--"u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest()
        {
            var expected = new List<byte[]>()
            {
                " "u8.ToArray(),
                " 1 "u8.ToArray(),
                " 2"u8.ToArray(),
                "3 "u8.ToArray(),
                "4"u8.ToArray(),
                " 5"u8.ToArray(),
                "  "u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                " "u8.ToArray(),
                " 1 "u8.ToArray(),
                " 2 3 "u8.ToArray(),
                "    "u8.ToArray(),
                "45"u8.ToArray(),
                "  "u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2 3 ,,    ,45,  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                index++;
            }

            index.Should().Be(0);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest6()
        {
            var index = 0;
            var expected = new List<byte[]>()
            {
                " 1"u8.ToArray(),
                "2 "u8.ToArray(),
                "3"u8.ToArray(),
                " "u8.ToArray(),
                " 4 "u8.ToArray(),
                "5"u8.ToArray(),
            };
            foreach (var s in Utf8Splitter.Split("-- 1--2 ----3-- -- 4 --5--"u8, "--"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest()
        {
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                "3"u8.ToArray(),
                "4"u8.ToArray(),
                "5"u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, 
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                "2 3"u8.ToArray(),
                "45"u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.Split(" , 1 , 2 3 ,,    ,45,  "u8, ","u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, ","u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                index++;
            }

            index.Should().Be(0);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest6()
        {
            var index = 0;
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                "3"u8.ToArray(),
                "4"u8.ToArray(),
                "5"u8.ToArray(),
            };
            foreach (var s in Utf8Splitter.Split("-- 1--2 ----3-- -- 4 --5--"u8, "--"u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitEmptyTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
        }

        [Fact]
        public void SplitEmptyTest2()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("1"u8, ""u8))
                {
                    index++;
                    s.ToArray().Should().Equal("1"u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("12"u8, ""u8))
                {
                    index++;
                    s.ToArray().Should().Equal("12"u8.ToArray());
                }

                index.Should().Be(1);
            }
        }

        [Fact]
        public void SplitEmptyWithTrimTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
        }

        [Fact]
        public void SplitEmptyWithTrimTest2()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ""u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().Should().Equal(""u8.ToArray());
                }

                index.Should().Be(1);
            }
        }

        [Fact]
        public void SplitEmptyWithRemoveEmptyEntriesTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
        }

        [Fact]
        public void SplitEmptyWithRemoveEmptyEntriesTest2()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ""u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                    s.ToArray().Should().Equal("  "u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                    s.ToArray().Should().Equal("  "u8.ToArray());
                }

                index.Should().Be(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                    s.ToArray().Should().Equal("  "u8.ToArray());
                }

                index.Should().Be(1);
            }
        }

        [Fact]
        public void SplitEmptyWithTrimEntriesAndRemoveEmptyEntriesTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
        }

        [Fact]
        public void SplitEmptyWithTrimEntriesAndRemoveEmptyEntriesTest2()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ""u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.Should().Be(0);
            }
        }


        [Fact]
        public void SplitAnyTest()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.SplitAny("1,2-3;4-5"u8, "-,;"u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(5);
        }

        [Fact]
        public void SplitAnyTest2()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.SplitAny("1,2,3,4,5"u8, ","u8))
            {
                s.Length.Should().Be(1);
                s[0].Should().Be(expected[index++]);
            }

            index.Should().Be(expected.Length);
        }

        [Fact]
        public void SplitAnyTest3()
        {
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                ""u8.ToArray(),
                "2"u8.ToArray(),
                ""u8.ToArray(),
                "3"u8.ToArray(),
                ""u8.ToArray(),
                "4"u8.ToArray(),
                ""u8.ToArray(),
                "5"u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny("1--2--3--4--5"u8, "--"u8))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitAnyTest4()
        {
            var index = 0;
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                "2"u8.ToArray(),
                "3"u8.ToArray(),
                " "u8.ToArray(),
                "4"u8.ToArray(),
                ""u8.ToArray(),
                "5"u8.ToArray(),
            };
            foreach (var s in Utf8Splitter.SplitAny("1,2-3; ,4-,5"u8, "-,;"u8))
            {
                s.SequenceEqual(expected[index++]).Should().BeTrue();
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitAnyTest5()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny("123456"u8, "-,;=:>?"u8))
            {
                index++;
                s.SequenceEqual("123456"u8).Should().BeTrue();
            }

            index.Should().Be(1);
        }

        [Fact]
        public void SplitAnyWithTrimEntriesTest()
        {
            {
                var expected = new List<byte[]>()
                {
                    ""u8.ToArray(),
                    "1"u8.ToArray(),
                    "2"u8.ToArray(),
                    "3"u8.ToArray(),
                    ""u8.ToArray(),
                    "4"u8.ToArray(),
                    "5"u8.ToArray(),
                    ""u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
            {
                var expected = new List<byte[]>()
                {
                    ""u8.ToArray(),
                    "1"u8.ToArray(),
                    "2"u8.ToArray(),
                    "3"u8.ToArray(),
                    ""u8.ToArray(),
                    "4"u8.ToArray(),
                    "5"u8.ToArray(),
                    ""u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, "-,;"u8, Utf8StringSplitOptions.TrimEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
        }

        [Fact]
        public void SplitAnyWithTrimEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                ""u8.ToArray(),
                "1"u8.ToArray(),
                "2 3"u8.ToArray(),
                ""u8.ToArray(),
                ""u8.ToArray(),
                "45"u8.ToArray(),
                ""u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2 3 ,,    ,45,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitAnyWithRemoveEmptyEntriesTest()
        {
            {
                var expected = new List<byte[]>()
                {
                    " "u8.ToArray(),
                    " 1 "u8.ToArray(),
                    " 2"u8.ToArray(),
                    "3 "u8.ToArray(),
                    "4"u8.ToArray(),
                    " 5"u8.ToArray(),
                    "  "u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
            {
                var expected = new List<byte[]>()
                {
                    " "u8.ToArray(),
                    " 1 "u8.ToArray(),
                    " 2"u8.ToArray(),
                    "3 "u8.ToArray(),
                    "4"u8.ToArray(),
                    " 5"u8.ToArray(),
                    "  "u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, "-,;"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
        }

        [Fact]
        public void SplitAnyWithRemoveEmptyEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                " "u8.ToArray(),
                " 1 "u8.ToArray(),
                " 2 3 "u8.ToArray(),
                "    "u8.ToArray(),
                "45"u8.ToArray(),
                "  "u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2 3 ,,    ,45,  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitAnyWithTrimEntriesAndRemoveEmptyEntriesTest()
        {
            {
                var expected = new List<byte[]>()
                {
                    "1"u8.ToArray(),
                    "2"u8.ToArray(),
                    "3"u8.ToArray(),
                    "4"u8.ToArray(),
                    "5"u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
            {
                var expected = new List<byte[]>()
                {
                    "1"u8.ToArray(),
                    "2"u8.ToArray(),
                    "3"u8.ToArray(),
                    "4"u8.ToArray(),
                    "5"u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2,3 ,,4, 5,  "u8, "-,;"u8, Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    s.ToArray().Should().Equal(expected[index++]);
                }

                index.Should().Be(expected.Count);
            }
        }

        [Fact]
        public void SplitAnyWithTrimEntriesAndRemoveEmptyEntriesTest2()
        {
            var expected = new List<byte[]>()
            {
                "1"u8.ToArray(),
                "2 3"u8.ToArray(),
                "45"u8.ToArray(),
            };
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny(" , 1 , 2 3 ,,    ,45,  "u8, ","u8, Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().Should().Equal(expected[index++]);
            }

            index.Should().Be(expected.Count);
        }

        [Fact]
        public void SplitAnyWithMultiByteDelimiterOptionsTest()
        {
            {
                var expected = new List<byte[]>()
                {
                    "‚ ‚¢"u8.ToArray(),
                    "‚¦"u8.ToArray(),
                    "‚¨"u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny("‚ ‚¢‚¤‚¦‚¤‚¨"u8, "‚¤"u8))
                {
                    s.SequenceEqual(expected[index++]).Should().BeTrue();
                }

                index.Should().Be(expected.Count);
            }
            {
                var expected = new List<byte[]>()
                {
                    "‚ ‚¢"u8.ToArray(),
                    "‚¦"u8.ToArray(),
                    "‚¨"u8.ToArray(),
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny("‚ ‚¢‚¤‚¦‚¤‚¨"u8, "‚¤"u8, delimiterOptions:Utf8StringDelimiterOptions.MultiByte))
                {
                    s.SequenceEqual(expected[index++]).Should().BeTrue();
                }

                index.Should().Be(expected.Count);
            }
        }

        [Fact]
        public void SplitAnyWithSingleByteDelimiterOptionsTest()
        {
            {
                var actual = new List<byte[]>();
                var expected = new List<byte[]>();
                expected.AddRange([[], [], [130], [], [132], [], [], [], [], [136], [], [], [], [], [138]]);

                var ss = "‚ ‚¢‚¤‚¦‚¤‚¨"u8.ToArray();
                foreach (var s in Utf8Splitter.SplitAny("‚ ‚¢‚¤‚¦‚¤‚¨"u8, "‚¤"u8, delimiterOptions: Utf8StringDelimiterOptions.SingleByte))
                {
                    actual.Add(s.ToArray());
                }

                actual.Should().BeEquivalentTo(expected);
            }
            {
                var expected = new List<byte>()
                {
                    130,
                    132,
                    136,
                    138,
                };
                var index = 0;

                foreach (var s in Utf8Splitter.SplitAny("‚ ‚¢‚¤‚¦‚¤‚¨"u8, "‚¤"u8, splitOptions: Utf8StringSplitOptions.RemoveEmptyEntries, delimiterOptions: Utf8StringDelimiterOptions.SingleByte))
                {
                    s.SequenceEqual([expected[index++]]).Should().BeTrue();
                }

                index.Should().Be(expected.Count);
            }
        }
    }
}