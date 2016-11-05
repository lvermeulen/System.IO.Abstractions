using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileGetLastWriteTimeUtcTests
    {
        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_GetLastWriteTimeUtc_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.GetLastWriteTimeUtc(path);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal(exception.ParamName, "path");
        }

        [Fact]
        public void MockFile_GetLastWriteTimeUtc_ShouldReturnDefaultTimeIfFileDoesNotExist()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            DateTime actualLastWriteTime = fileSystem.File.GetLastWriteTimeUtc(@"c:\does\not\exist.txt");

            // Assert
            Assert.Equal(new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc), actualLastWriteTime);
        }
    }
}