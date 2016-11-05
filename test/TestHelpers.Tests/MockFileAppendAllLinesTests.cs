using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileAppendAllLinesTests
    {
        [Fact]
        public void MockFile_AppendAllLines_ShouldPersistNewLinesToExistingFile()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            // Assert
            Assert.Equal(
                "Demo text contentline 1" + Environment.NewLine + "line 2" + Environment.NewLine + "line 3" + Environment.NewLine,
                file.ReadAllText(path));
        }

        [Fact]
        public void MockFile_AppendAllLines_ShouldPersistNewLinesToNewFile()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\"), new MockDirectoryData() }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            // Assert
            Assert.Equal(
                "line 1" + Environment.NewLine + "line 2" + Environment.NewLine + "line 3" + Environment.NewLine,
                file.ReadAllText(path));
        }

        [Fact]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathIsZeroLength()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.AppendAllLines(string.Empty, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.AppendAllLines(path, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData("\"")]
        [InlineData("<")]
        [InlineData(">")]
        [InlineData("|")]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathContainsInvalidChar(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.AppendAllLines(path, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockFile_AppendAllLines_ShouldThrowArgumentNullExceptionIfContentIsNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.AppendAllLines("foo", null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal(exception.ParamName, "contents");
        }

        [Fact]
        public void MockFile_AppendAllLines_ShouldThrowArgumentNullExceptionIfEncodingIsNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.AppendAllLines("foo.txt", new [] { "bar" }, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal(exception.ParamName, "encoding");
        }
    }
}
