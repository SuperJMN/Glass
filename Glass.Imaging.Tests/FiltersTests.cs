namespace Glass.Imaging.Tests
{
    using Xunit;
    using ZoneConfigurations.Alpha;
    using ZoneConfigurations.Alphanumeric;
    using ZoneConfigurations.Numeric;


    public class FiltersTests
    {
        public static string MailRegex { get; } = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                                  + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                                  + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                                  + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";


        [Theory]
        [InlineData("111 11 1\r\n123456", 0, int.MaxValue, "123456")]
        [InlineData("123456\r\n1 1 11 1", 6, 6, "123456")]
        [InlineData("123456\r\n1 1 11 1", 4, 7, "123456")]
        [InlineData("123456\r\nI I I I I I I I I I", 10, 10, "123456")]
        [InlineData("123456\r\nIIIIIIIIII", 10, 10, "123456")]
        [InlineData("I23A56\r\n||||1| ||||||", 0, 6, "I23A56")]
        [InlineData("111 1 \r\n1 1 111 11 1\r\nI23A567\r\n", 6, 6, "I23A567")]
        [InlineData("12\r\nAB3456", 4, 7, "AB3456")]
        [InlineData(" 123456 ", 0, int.MaxValue, "123456")]
        [InlineData(" 12345556 \r\n  2121", 4, 5, "2121")]
        [InlineData("18 8\r\n1101118110 988 5085\r\n11", 4, 5, "5085")]
        [InlineData(null, 0, int.MaxValue, null)]
        public void NumericTest(string str, int min, int max, string expected)
        {
            var sut = new NumericStringFilter { MinLength = min, MaxLength = max };
            var actual = sut.Filter(str);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("abc\r\nabcdefh", 6, 8, "abcdefh", null)]
        [InlineData("abc\r\nabcdefh", 2, 3, "abc", null)]
        [InlineData("abc abcdefh", 2, 3, "abc", null)]
        [InlineData("pericopalotes.com perico@palotes.com", 0, short.MaxValue, "perico@palotes.com", @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$")]
        public void AlphanumericTest(string str, int min, int max, string expected, string regex)
        {
            var sut = new AlphanumericStringFilter { MinLength = min, MaxLength = max, Regex = regex };
            var actual = sut.Filter(str);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData("abc\r\n123", 0, int.MaxValue, "abc", null)]
        [InlineData("abc34 abcd4", 5, 5, "abcd4", null)]
        [InlineData("abcd dddd", 0, int.MaxValue, "dddd", "d*")]
        [InlineData("ddddd ddd", 0, int.MaxValue, "ddd", "d{3}")]
        [InlineData("a b", 0, int.MaxValue, "b", "b")]

        public void AlphaTest(string str, int min, int max, string expected, string regex = null)
        {
            var sut = new AlphaOnlyStringFilter { MinLength = min, MaxLength = max, Regex = regex };
            var actual = sut.Filter(str);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("500\r\n2000\r\n4000", "2000")]
        public void IntervalTest(string str, string expected)
        {
            var sut = new NumericStringFilter { Minimum = 1000, Maximum = 3000 };
            var actual = sut.Filter(str);
            Assert.Equal(expected, actual);
        }
    }
}
