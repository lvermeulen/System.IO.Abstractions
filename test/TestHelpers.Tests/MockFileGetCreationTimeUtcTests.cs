using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileGetCreationTimeUtcTests
    {
        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_GetCreationTimeUtc_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.GetCreationTimeUtc(path);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal(exception.ParamName, "path");
        }

        [Fact]
        public void MockFile_GetCreationTimeUtc_ShouldReturnDefaultTimeIfFileDoesNotExist()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            DateTime actualCreationTime = fileSystem.File.GetCreationTimeUtc(@"c:\does\not\exist.txt");

            // Assert
            Assert.Equal(new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc), actualCreationTime);
        }
    }
}
