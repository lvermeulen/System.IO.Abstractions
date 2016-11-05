using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileStreamTests
    {
        [Fact]
        public void MockFileStream_Flush_WritesByteToFile()
        {
            // Arrange
            string filepath = XFS.Path(@"c:\something\foo.txt");
            MockFileSystem filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            MockFileStream cut = new MockFileStream(filesystem, filepath);

            // Act
            cut.WriteByte(255);
            cut.Flush();

            // Assert
            Assert.Equal(new byte[]{255}, filesystem.GetFile(filepath).Contents);
        }

        [Fact]
        public void MockFileStream_Dispose_ShouldNotResurrectFile()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            string path = XFS.Path("C:\\test");
            string directory = fileSystem.Path.GetDirectoryName(path);
            fileSystem.AddFile(path, new MockFileData("Bla"));
            Stream stream = fileSystem.File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete);

            int fileCount1 = fileSystem.Directory.GetFiles(directory, "*").Length;
            fileSystem.File.Delete(path);
            int fileCount2 = fileSystem.Directory.GetFiles(directory, "*").Length;
            stream.Dispose();
            int fileCount3 = fileSystem.Directory.GetFiles(directory, "*").Length;

            Assert.Equal(1, fileCount1);
            Assert.Equal(0, fileCount2);
            Assert.Equal(0, fileCount3);
        }
    }
}
