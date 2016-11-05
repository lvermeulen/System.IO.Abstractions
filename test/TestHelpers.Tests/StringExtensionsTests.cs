using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class StringExtensions
    {
        [Fact]
        public void SplitLines_InputWithOneLine_ShouldReturnOnlyOneLine()
        {
            string input = "This is row one";
            string[] expected = new[] { "This is row one" };

            string[] result = input.SplitLines();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void SplitLines_InputWithTwoLinesSeparatedWithLf_ShouldReturnBothLines() {
            string input = "This is row one\nThis is row two";
            string[] expected = new[] { "This is row one", "This is row two" };

            string[] result = input.SplitLines();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void SplitLines_InputWithTwoLinesSeparatedWithCr_ShouldReturnBothLines() {
            string input = "This is row one\rThis is row two";
            string[] expected = new[] { "This is row one", "This is row two" };

            string[] result = input.SplitLines();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void SplitLines_InputWithTwoLinesSeparatedWithCrLf_ShouldReturnBothLines() {
            string input = "This is row one\r\nThis is row two";
            string[] expected = new[] { "This is row one", "This is row two" };

            string[] result = input.SplitLines();

            Assert.Equal(result, expected);
        }

        [Fact]
        public void SplitLines_InputWithTwoLinesSeparatedWithAllLineEndings_ShouldReturnAllLines() {
            string input = "one\r\ntwo\rthree\nfour";
            string[] expected = new[] { "one", "two", "three", "four" };

            string[] result = input.SplitLines();

            Assert.Equal(result, expected);
        }

    }
}
