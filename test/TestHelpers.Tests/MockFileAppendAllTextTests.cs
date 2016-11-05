using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileAppendAllTextTests
    {
        [Fact]
        public void MockFile_AppendAllText_ShouldPersistNewText()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {path, new MockFileData("Demo text content")}
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            file.AppendAllText(path, "+ some text");

            // Assert
            Assert.Equal(
                "Demo text content+ some text",
                file.ReadAllText(path));
        }

        [Fact]
        public void MockFile_AppendAllText_ShouldPersistNewTextWithDifferentEncoding()
        {
            // Arrange
            const string PATH = @"c:\something\demo.txt";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {PATH, new MockFileData("AA", Encoding.UTF32)}
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            file.AppendAllText(PATH, "BB", Encoding.UTF8);

            // Assert
            Assert.Equal(
                new byte[] {255, 254, 0, 0, 65, 0, 0, 0, 65, 0, 0, 0, 66, 66},
                fileSystem.GetFile(PATH).Contents);
        }

        [Fact]
        public void MockFile_AppendAllText_ShouldCreateIfNotExist()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {path, new MockFileData("Demo text content")}
            });

            // Act
            fileSystem.File.AppendAllText(path, " some text");

            // Assert
            Assert.Equal(
                "Demo text content some text",
                fileSystem.File.ReadAllText(path));
        }

        [Fact]
        public void MockFile_AppendAllText_ShouldCreateIfNotExistWithBom()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            const string PATH = @"c:\something\demo3.txt";
            fileSystem.AddDirectory(@"c:\something\");

            // Act
            fileSystem.File.AppendAllText(PATH, "AA", Encoding.UTF32);

            // Assert
            Assert.Equal(
                new byte[] {255, 254, 0, 0, 65, 0, 0, 0, 65, 0, 0, 0},
                fileSystem.GetFile(PATH).Contents);
        }

        [Fact]
        public void MockFile_AppendAllText_ShouldFailIfNotExistButDirectoryAlsoNotExist()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {path, new MockFileData("Demo text content")}
            });

            // Act
            path = XFS.Path(@"c:\something2\demo.txt");

            // Assert
            Exception ex = Assert.Throws<DirectoryNotFoundException>(() => fileSystem.File.AppendAllText(path, "some text"));
            Assert.Equal(ex.Message, $"Could not find a part of the path '{path}'.");

            ex = Assert.Throws<DirectoryNotFoundException>(() => fileSystem.File.AppendAllText(path, "some text", Encoding.Unicode));
            Assert.Equal(ex.Message, $"Could not find a part of the path '{path}'.");
        }

        [Fact]
        public void MockFile_AppendAllText_ShouldPersistNewTextWithCustomEncoding()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {path, new MockFileData("Demo text content")}
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            file.AppendAllText(path, "+ some text", Encoding.BigEndianUnicode);

            // Assert
            byte[] expected = new byte[]
            {
                68, 101, 109, 111, 32, 116, 101, 120, 116, 32, 99, 111, 110, 116,
                101, 110, 116, 0, 43, 0, 32, 0, 115, 0, 111, 0, 109, 0, 101,
                0, 32, 0, 116, 0, 101, 0, 120, 0, 116
            };

            if (XFS.IsUnixPlatform())
            {
                // Remove EOF on mono
                expected = new byte[]
                {
                    68, 101, 109, 111, 32, 116, 101, 120, 116, 32, 99, 111, 110, 116,
                    101, 110, 0, 43, 0, 32, 0, 115, 0, 111, 0, 109, 0, 101,
                    0, 32, 0, 116, 0, 101, 0, 120, 0, 116
                };
            }

            Assert.Equal(
                expected,
                file.ReadAllBytes(path));
        }
    }
}