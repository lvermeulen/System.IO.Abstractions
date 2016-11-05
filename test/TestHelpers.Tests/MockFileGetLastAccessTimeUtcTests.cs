using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileGetLastAccessTimeUtcTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_GetLastAccessTimeUtc_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.GetLastAccessTimeUtc(path);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal(exception.ParamName, "path");
        }

        [Fact]
        public void MockFile_GetLastAccessTimeUtc_ShouldReturnDefaultTimeIfFileDoesNotExist()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            DateTime actualLastAccessTime = fileSystem.File.GetLastAccessTimeUtc(@"c:\does\not\exist.txt");

            // Assert
            Assert.Equal(new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc), actualLastAccessTime);
        }
    }
}
