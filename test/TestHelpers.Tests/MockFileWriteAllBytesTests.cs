using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileWriteAllBytesTests
    {
        [Fact]
        public void MockFile_WriteAllBytes_ShouldWriteDataToMemoryFileSystem()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();
            byte[] fileContent = new byte[] { 1, 2, 3, 4 };

            // Act
            fileSystem.File.WriteAllBytes(path, fileContent);

            // Assert
            Assert.Equal(
                fileContent,
                fileSystem.GetFile(path).Contents);
        }

        [Fact]
        public void MockFile_WriteAllBytes_ShouldThrowAnUnauthorizedAccessExceptionIfFileIsHidden()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { path, new MockFileData("this is hidden") },
                });
            fileSystem.File.SetAttributes(path, FileAttributes.Hidden);

            // Act
            Action action = () => fileSystem.File.WriteAllBytes(path, new byte[] { 123 });

            // Assert
            Assert.Throws<UnauthorizedAccessException>(action);
        }

        [Fact]
        public void MockFile_WriteAllBytes_ShouldThrowAnArgumentExceptionIfContainsIllegalCharacters()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.WriteAllBytes("<<<", new byte[] { 123 });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockFile_WriteAllBytes_ShouldThrowAnArgumentNullExceptionIfContainsIllegalCharacters()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.WriteAllBytes(null, new byte[] { 123 });

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Path cannot be null.", exception.Message);
            Assert.Equal(exception.ParamName, "path");
        }

        [Fact]
        public void MockFile_WriteAllBytes_ShouldThrowAnArgumentNullExceptionIfBytesAreNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            string path = XFS.Path(@"c:\something\demo.txt");

            // Act
            Action action = () => fileSystem.File.WriteAllBytes(path, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Value cannot be null.", exception.Message);
            Assert.Equal(exception.ParamName, "bytes");
        }
    }
}
