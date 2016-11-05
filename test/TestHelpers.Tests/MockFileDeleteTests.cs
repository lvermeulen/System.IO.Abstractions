using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileDeleteTests
    {
        [Fact]
        public void MockFile_Delete_ShouldDeleteFile()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            string path = XFS.Path("C:\\test");
            string directory = fileSystem.Path.GetDirectoryName(path);
            fileSystem.AddFile(path, new MockFileData("Bla"));

            int fileCount1 = fileSystem.Directory.GetFiles(directory, "*").Length;
            fileSystem.File.Delete(path);
            int fileCount2 = fileSystem.Directory.GetFiles(directory, "*").Length;

            Assert.Equal(1, fileCount1);
            Assert.Equal(0, fileCount2);
        }

        [InlineData(" ")]
        [InlineData("   ")]
        public void MockFile_Delete_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.Delete(path);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }
    }
}