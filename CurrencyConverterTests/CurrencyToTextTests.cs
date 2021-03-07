using CurrencyConverterServer.Converters;
using System;
using Xunit;

namespace CurrencyConverterTests
{
    public class CurrencyToTextTests
    {
        [Theory]
        [InlineData("0", "zero dollars")]
        [InlineData("000000", "zero dollars")]
        [InlineData("1", "one dollar")]
        [InlineData("000001", "one dollar")]
        [InlineData("25,1", "twenty-five dollars and ten cents")]
        [InlineData("25,10", "twenty-five dollars and ten cents")]
        [InlineData("00000000000000025,10", "twenty-five dollars and ten cents")]
        [InlineData("25,10000", "twenty-five dollars and ten cents")]
        [InlineData("0,01", "zero dollars and one cent")]
        [InlineData("45 100", "forty-five thousand one hundred dollars")]
        [InlineData("45 000", "forty-five thousand dollars")]
        [InlineData("45  0 0 0 ", "forty-five thousand dollars")]
        [InlineData("999 999 999,99", "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents")]
        public void Converts_Valid_Values(string input, string expected)
        {
            var converter = new NumberToTextConverter();
            var result = converter.Convert(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("1$")]
        [InlineData("2,5,1")]
        [InlineData("2,5134")]
        [InlineData("25,1000,")]
        [InlineData("45   h100")]
        [InlineData("-45 000")]
        [InlineData("999 999 999 999,99")]
        public void Throws_Exception_When_Input_Is_Invalid(string input)
        {
            var converter = new NumberToTextConverter();
            Assert.Throws<ArgumentException>(() => converter.Convert(input));
        }
    }
}
