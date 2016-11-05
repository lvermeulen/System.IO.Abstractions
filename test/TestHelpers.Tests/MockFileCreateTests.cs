using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileCreateTests
    {
        [Fact]
        public void Mockfile_Create_ShouldCreateNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            MockFile sut = new MockFile(fileSystem);

            Assert.False(fileSystem.FileExists(fullPath));

            sut.Create(fullPath).Dispose();

            Assert.True(fileSystem.FileExists(fullPath));
        }

        [Fact]
        public void Mockfile_Create_CanWriteToNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();
            byte[] data = new UTF8Encoding(false).GetBytes("Test string");

            MockFile sut = new MockFile(fileSystem);
            using (Stream stream = sut.Create(fullPath))
            {
                stream.Write(data, 0, data.Length);
            }

            MockFileData mockFileData = fileSystem.GetFile(fullPath);
            byte[] fileData = mockFileData.Contents;

            Assert.Equal(fileData, data);
        }

        [Fact]
        public void Mockfile_Create_OverwritesExistingFile()
        {
            string path = XFS.Path(@"c:\some\file.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            MockFile mockFile = new MockFile(fileSystem);

            // Create a file
            using (Stream stream = mockFile.Create(path))
            {
                byte[] contents = new UTF8Encoding(false).GetBytes("Test 1");
                stream.Write(contents, 0, contents.Length);
            }

            // Create new file that should overwrite existing file
            byte[] expectedContents = new UTF8Encoding(false).GetBytes("Test 2");
            using (Stream stream = mockFile.Create(path))
            {
                stream.Write(expectedContents, 0, expectedContents.Length);
            }

            byte[] actualContents = fileSystem.GetFile(path).Contents;

            Assert.Equal(actualContents, expectedContents);
        }

        [Fact]
        public void Mockfile_Create_ShouldThrowUnauthorizedAccessExceptionIfPathIsReadOnly()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\read-only.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { path, new MockFileData("Content") } });
            MockFile mockFile = new MockFile(fileSystem);

            // Act
            mockFile.SetAttributes(path, FileAttributes.ReadOnly);

            // Assert
            UnauthorizedAccessException exception =  Assert.Throws<UnauthorizedAccessException>(() => mockFile.Create(path).Dispose());
            Assert.Equal(exception.Message, $"Access to the path '{path}' is denied.");
        }

        [Fact]
        public void Mockfile_Create_ShouldThrowArgumentExceptionIfPathIsZeroLength()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.Create("");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData("\"")]
        [InlineData("<")]
        [InlineData(">")]
        [InlineData("|")]
        public void MockFile_Create_ShouldThrowArgumentNullExceptionIfPathIsNull1(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.Create(path);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_Create_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.Create(path);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockFile_Create_ShouldThrowArgumentNullExceptionIfPathIsNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.Create(null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Path cannot be null.", exception.Message);
        }
    }
}