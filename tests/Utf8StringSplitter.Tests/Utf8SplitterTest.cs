using Shouldly;
using System;
using System.Diagnostics;
using System.Text;

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
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
        }

        [Fact]
        public void SplitTest2()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, (byte)','))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                index++;
                s.ToArray().ShouldBe(""u8.ToArray());
            }

            index.ShouldBe(1);
        }

        [Fact]
        public void SplitWithTrimEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8, Utf8StringSplitOptions.TrimEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitWithTrimEntriesTest7()
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
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, (byte)',', Utf8StringSplitOptions.TrimEntries))
            {
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                index++;
            }

            index.ShouldBe(0);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitWithRemoveEmptyEntriesTest7()
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
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, (byte)',', Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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

            index.ShouldBe(0);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest4()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1,2,3,4,5"u8, ","u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
        }

        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest5()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.Split("1--2--3--4--5"u8, "--"u8,
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
        }


        [Fact]
        public void SplitWithTrimEntriesAndRemoveEmptyEntriesTest7()
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
            foreach (var s in Utf8Splitter.Split(" , 1 , 2,3 ,,4, 5,  "u8, (byte)',',
                Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
            {
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitEmptyTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
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
                    s.ToArray().ShouldBe("1"u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("12"u8, ""u8))
                {
                    index++;
                    s.ToArray().ShouldBe("12"u8.ToArray());
                }

                index.ShouldBe(1);
            }
        }

        [Fact]
        public void SplitEmptyTest3()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.Split(""u8, (byte)','))
            {
                index++;
                s.ToArray().ShouldBe(""u8.ToArray());
            }

            index.ShouldBe(1);
        }

        [Fact]
        public void SplitEmptyWithTrimTest()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ""u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
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
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(""u8.ToArray());
                }

                index.ShouldBe(1);
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

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
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
                    s.ToArray().ShouldBe("  "u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                    s.ToArray().ShouldBe("  "u8.ToArray());
                }

                index.ShouldBe(1);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8, Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                    s.ToArray().ShouldBe("  "u8.ToArray());
                }

                index.ShouldBe(1);
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

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, ","u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split(""u8, "---"u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
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

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, ","u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, "---"u8,
                    Utf8StringSplitOptions.TrimEntries | Utf8StringSplitOptions.RemoveEmptyEntries))
                {
                    index++;
                }

                index.ShouldBe(0);
            }
        }

        [Fact]
        public void SplitWhiteSpace()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, " "u8))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(3);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, " "u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(3);

                index = 0;
                foreach (var s in Utf8Splitter.Split("     "u8, " "u8, Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(6);

                var expected = new List<byte[]>()
                {
                    ""u8.ToArray(),
                    ""u8.ToArray(),
                    "1"u8.ToArray(),
                    ""u8.ToArray(),
                    ""u8.ToArray(),
                };
                index = 0;
                foreach (var s in Utf8Splitter.Split("  1  "u8, " "u8, Utf8StringSplitOptions.TrimEntries))
                {
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(5);
            }
        }

        [Fact]
        public void SplitWhiteSpace2()
        {
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, (byte)' '))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(3);
            }
            {
                var index = 0;
                foreach (var s in Utf8Splitter.Split("  "u8, (byte)' ', Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(3);

                index = 0;
                foreach (var s in Utf8Splitter.Split("     "u8, (byte)' ', Utf8StringSplitOptions.TrimEntries))
                {
                    index++;
                    s.ToArray().ShouldBe(Array.Empty<byte>());
                }

                index.ShouldBe(6);

                var expected = new List<byte[]>()
                {
                    ""u8.ToArray(),
                    ""u8.ToArray(),
                    "1"u8.ToArray(),
                    ""u8.ToArray(),
                    ""u8.ToArray(),
                };
                index = 0;
                foreach (var s in Utf8Splitter.Split("  1  "u8, (byte)' ', Utf8StringSplitOptions.TrimEntries))
                {
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(5);
            }
        }

        [Fact]
        public void SplitAnyTest()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.SplitAny("1,2-3;4-5"u8, "-,;"u8))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(5);
        }

        [Fact]
        public void SplitAnyTest2()
        {
            var index = 0;
            var expected = "12345"u8.ToArray();
            foreach (var s in Utf8Splitter.SplitAny("1,2,3,4,5"u8, ","u8))
            {
                s.Length.ShouldBe(1);
                s[0].ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Length);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
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
                s.SequenceEqual(expected[index++]).ShouldBeTrue();
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitAnyTest5()
        {
            var index = 0;
            foreach (var s in Utf8Splitter.SplitAny("123456"u8, "-,;=:>?"u8))
            {
                index++;
                s.Length.ShouldBe(6);
                s.SequenceEqual("123456"u8).ShouldBeTrue();
            }

            index.ShouldBe(1);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                    s.ToArray().ShouldBe(expected[index++]);
                }

                index.ShouldBe(expected.Count);
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
                s.ToArray().ShouldBe(expected[index++]);
            }

            index.ShouldBe(expected.Count);
        }

        [Fact]
        public void SplitAnyWithUtf8SeparatorOptionsTest()
        {
            var source = new byte[] { 227, 129, 130, 227, 129, 132, 227, 129, 134, 227, 129, 136, 227, 129, 134, 227, 129, 138 }; //‚ ‚¢‚¤‚¦‚¤‚¨
            var separator = new byte[] { 227, 129, 134 }; //‚¤
            {
                var expected = new List<byte[]>()
                {
                    new byte[] { 227, 129, 130, 227, 129, 132 }, // ‚ ‚¢
                    new byte[] { 227, 129, 136}, // ‚¦
                    new byte[] { 227, 129, 138}, // ‚¨
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(source, separator))
                {
                    s.SequenceEqual(expected[index++]).ShouldBeTrue();
                }

                index.ShouldBe(expected.Count);
            }
            {
                var expected = new List<byte[]>()
                {
                    new byte[] { 227, 129, 130, 227, 129, 132 },
                    new byte[] { 227, 129, 136},
                    new byte[] { 227, 129, 138},
                };
                var index = 0;
                foreach (var s in Utf8Splitter.SplitAny(source, separator, separatorOptions:Utf8StringSeparatorOptions.Utf8))
                {
                    s.SequenceEqual(expected[index++]).ShouldBeTrue();
                }

                index.ShouldBe(expected.Count);
            }
        }

        [Fact]
        public void SplitAnyWithBytesSeparatorOptionsTest()
        {
            var source = new byte[] { 227, 129, 130, 227, 129, 132, 227, 129, 134, 227, 129, 136, 227, 129, 134, 227, 129, 138 }; //‚ ‚¢‚¤‚¦‚¤‚¨
            var separator = new byte[] { 227, 129, 134 }; //‚¤
            {
                var actual = new List<byte[]>();
                var expected = new List<byte[]>();
                expected.AddRange([[], [], [130], [], [132], [], [], [], [], [136], [], [], [], [], [138]]);
                foreach (var s in Utf8Splitter.SplitAny(source, separator, separatorOptions: Utf8StringSeparatorOptions.Bytes))
                {
                    actual.Add(s.ToArray());
                }

                actual.ShouldBeEquivalentTo(expected);
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

                foreach (var s in Utf8Splitter.SplitAny(source, separator, splitOptions: Utf8StringSplitOptions.RemoveEmptyEntries, separatorOptions: Utf8StringSeparatorOptions.Bytes))
                {
                    s.SequenceEqual([expected[index++]]).ShouldBeTrue();
                }

                index.ShouldBe(expected.Count);
            }
        }

        [Fact]
        public void ToArrayTest()
        {
            var expected = new byte[][] { "1"u8.ToArray(), "2"u8.ToArray(), "3"u8.ToArray(), "4"u8.ToArray(), "5"u8.ToArray() };
            var result = Utf8Splitter.Split("1,2,3,4,5"u8, ","u8).ToArray();

            for (var i = 0; i < result.Length; i++)
            {
                expected[i].ShouldBe(result[i]);
            }
            result.Length.ShouldBe(expected.Length);
        }

        [Fact]
        public void ToArrayTest2()
        {
            var expected = new byte[][] { "1"u8.ToArray(), "2"u8.ToArray(), "3"u8.ToArray(), "4"u8.ToArray(), "5"u8.ToArray() };
            var result = Utf8Splitter.SplitAny("1,2-3;4-5"u8, "-,;"u8).ToArray();

            for (var i = 0; i < result.Length; i++)
            {
                expected[i].ShouldBe(result[i]);
            }
            result.Length.ShouldBe(expected.Length);
        }

        [Fact]
        public void ToUtf16ArrayTest()
        {
            var expected = "12345".Select(c => c.ToString()).ToArray();
            var result = Utf8Splitter.Split("1,2,3,4,5"u8, ","u8).ToUtf16Array();

            for (var i = 0; i < result.Length; i++)
            {
                expected[i].ShouldBe(result[i]);
            }
            result.Length.ShouldBe(expected.Length);
        }

        [Fact]
        public void ToUtf16ArrayTest2()
        {
            var expected = "12345".Select(c => c.ToString()).ToArray();
            var result = Utf8Splitter.SplitAny("1,2-3;4-5"u8, "-,;"u8).ToUtf16Array();

            for (var i = 0; i < result.Length; i++)
            {
                expected[i].ShouldBe(result[i]);
            }
            result.Length.ShouldBe(expected.Length);
        }

    }
}