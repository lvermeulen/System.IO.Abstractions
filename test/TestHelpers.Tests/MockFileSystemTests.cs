using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Xml.Serialization;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileSystemTests
    {
        [Fact]
        public void MockFileSystem_GetFile_ShouldReturnNullWhenFileIsNotRegistered()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\something\demo.txt", new MockFileData("Demo\r\ntext\ncontent\rvalue") },
                { @"c:\something\other.gif", new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            // Act
            MockFileData result = fileSystem.GetFile(@"c:\something\else.txt");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MockFileSystem_GetFile_ShouldReturnFileRegisteredInConstructor()
        {
            // Arrange
            MockFileData file1 = new MockFileData("Demo\r\ntext\ncontent\rvalue");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\something\demo.txt", file1 },
                { @"c:\something\other.gif", new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            // Act
            MockFileData result = fileSystem.GetFile(@"c:\something\demo.txt");

            // Assert
            Assert.Equal(file1, result);
        }

        [Fact]
        public void MockFileSystem_GetFile_ShouldReturnFileRegisteredInConstructorWhenPathsDifferByCase()
        {
            // Arrange
            MockFileData file1 = new MockFileData("Demo\r\ntext\ncontent\rvalue");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\something\demo.txt", file1 },
                { @"c:\something\other.gif", new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            // Act
            MockFileData result = fileSystem.GetFile(@"c:\SomeThing\DEMO.txt");

            // Assert
            Assert.Equal(file1, result);
        }

        [Fact]
        public void MockFileSystem_AddFile_ShouldRepaceExistingFile()
        {
            const string PATH = @"c:\some\file.txt";
            const string EXISTING_CONTENT = "Existing content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PATH, new MockFileData(EXISTING_CONTENT) }
            });
            Assert.Equal(fileSystem.GetFile(PATH).TextContents, EXISTING_CONTENT);

            const string NEW_CONTENT = "New content";
            fileSystem.AddFile(PATH, new MockFileData(NEW_CONTENT));

            Assert.Equal(fileSystem.GetFile(PATH).TextContents, NEW_CONTENT);
        }

        [Fact]
        public void Is_Serializable()
        {
            MockFileData file1 = new MockFileData("Demo\r\ntext\ncontent\rvalue");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\something\demo.txt", file1 },
                { @"c:\something\other.gif", new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });
            MemoryStream memoryStream = new MemoryStream();

            var serializer = new XmlSerializer(fileSystem.GetType());
            serializer.Serialize(memoryStream, fileSystem);

            Assert.True(memoryStream.Length > 0, "Length didn't increase after serialization task.");
        }

        [Fact]
        public void MockFileSystem_AddDirectory_ShouldCreateDirectory()
        {
            // Arrange
            string baseDirectory = MockUnixSupport.Path(@"C:\Test");
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            fileSystem.AddDirectory(baseDirectory);

            // Assert
            Assert.True(fileSystem.Directory.Exists(baseDirectory));
        }

        [Fact]
        public void MockFileSystem_AddDirectory_ShouldThrowExceptionIfDirectoryIsReadOnly()
        {
            // Arrange
            string baseDirectory = MockUnixSupport.Path(@"C:\Test");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(baseDirectory, new MockFileData(string.Empty));
            fileSystem.File.SetAttributes(baseDirectory, FileAttributes.ReadOnly);

            // Act
            Action act = () => fileSystem.AddDirectory(baseDirectory);

            // Assert
            Assert.Throws<UnauthorizedAccessException>(act);
        }

        [Fact]
        public void MockFileSystem_DriveInfo_ShouldNotThrowAnyException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(MockUnixSupport.Path(@"C:\Test"));
            fileSystem.AddDirectory(MockUnixSupport.Path(@"Z:\Test"));
            fileSystem.AddDirectory(MockUnixSupport.Path(@"d:\Test"));

            // Act
            DriveInfoBase[] actualResults = fileSystem.DriveInfo.GetDrives();

            // Assert
            Assert.NotNull(actualResults);
        }
    }
}