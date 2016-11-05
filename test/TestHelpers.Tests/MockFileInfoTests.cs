using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileInfoTests
    {
        [Fact]
        public void MockFileInfo_Exists_ShouldReturnTrueIfFileExistsInMemoryFileSystem()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData("Demo text content") },
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            bool result = fileInfo.Exists;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MockFileInfo_Exists_ShouldReturnFalseIfFileDoesNotExistInMemoryFileSystem()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData("Demo text content") },
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\foo.txt"));

            // Act
            bool result = fileInfo.Exists;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockFileInfo_Length_ShouldReturnLengthOfFileInMemoryFileSystem()
        {
            // Arrange
            const string FILE_CONTENT = "Demo text content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData(FILE_CONTENT) },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData(FILE_CONTENT) },
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            long result = fileInfo.Length;

            // Assert
            Assert.Equal(FILE_CONTENT.Length, result);
        }

        [Fact]
        public void MockFileInfo_Length_ShouldThrowFileNotFoundExceptionIfFileDoesNotExistInMemoryFileSystem()
        {
            // Arrange
            const string FILE_CONTENT = "Demo text content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData(FILE_CONTENT) },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData(FILE_CONTENT) },
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\foo.txt"));

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() => fileInfo.Length.ToString(CultureInfo.InvariantCulture));
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
            Assert.Equal(XFS.Path(@"c:\foo.txt"), ex.FileName);
        }

        [Fact]
        public void MockFileInfo_CreationTimeUtc_ShouldReturnCreationTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime creationTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime result = fileInfo.CreationTimeUtc;

            // Assert
            Assert.Equal(creationTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFileInfo_CreationTimeUtc_ShouldSetCreationTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime creationTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime newUtcTime = DateTime.UtcNow;
            fileInfo.CreationTimeUtc = newUtcTime;

            // Assert
            Assert.Equal(newUtcTime, fileInfo.CreationTimeUtc);
        }


        [Fact]
        public void MockFileInfo_CreationTime_ShouldReturnCreationTimeOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime creationTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime result = fileInfo.CreationTime;

            // Assert
            Assert.Equal(creationTime, result);
        }

        [Fact]
        public void MockFileInfo_CreationTime_ShouldSetCreationTimeOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime creationTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime newTime = DateTime.Now;
            fileInfo.CreationTime = newTime;

            // Assert
            Assert.Equal(newTime, fileInfo.CreationTime);
        }

        [Fact]
        public void MockFileInfo_IsReadOnly_ShouldSetReadOnlyAttributeOfFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt")) {IsReadOnly = true};

            // Act

            // Assert
            Assert.Equal(FileAttributes.ReadOnly, fileData.Attributes & FileAttributes.ReadOnly);
        }

        [Fact]
        public void MockFileInfo_IsReadOnly_ShouldSetNotReadOnlyAttributeOfFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content") {Attributes = FileAttributes.ReadOnly};
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });

            // Act
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt")) { IsReadOnly = false };

            // Assert
            Assert.NotEqual(FileAttributes.ReadOnly, fileData.Attributes & FileAttributes.ReadOnly);
        }

        [Fact]
        public void MockFileInfo_AppendText_ShouldAddTextToFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            using (StreamWriter file = fileInfo.AppendText())
            {
                file.WriteLine("This should be at the end");
            }

            string newcontents;
            using (StreamReader newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.Equal("Demo text contentThis should be at the end\r\n", newcontents);
        }

        [Fact]
        public void MockFileInfo_OpenWrite_ShouldAddDataToFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));
            byte[] bytesToAdd = new byte[] {65, 66, 67, 68, 69};

            // Act
            using (Stream file = fileInfo.OpenWrite())
            {
                file.Write(bytesToAdd, 0, bytesToAdd.Length);
            }

            string newcontents;
            using (StreamReader newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.Equal("ABCDEtext content", newcontents);
        }

        [Fact]
        public void MockFileInfo_Encrypt_ShouldReturnXorOfFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            fileInfo.Encrypt();

            string newcontents;
            using (StreamReader newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.NotEqual("Demo text content", newcontents);
        }

        [Fact]
        public void MockFileInfo_Decrypt_ShouldReturnCorrectContentsFileInMemoryFileSystem()
        {
            // Arrange
            MockFileData fileData = new MockFileData("Demo text content");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));
            fileInfo.Encrypt();

            // Act
            fileInfo.Decrypt();

            string newcontents;
            using (StreamReader newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.Equal("Demo text content", newcontents);
        }

        [Fact]
        public void MockFileInfo_LastAccessTimeUtc_ShouldReturnLastAccessTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime lastAccessTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { LastAccessTime = lastAccessTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime result = fileInfo.LastAccessTimeUtc;

            // Assert
            Assert.Equal(lastAccessTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFileInfo_LastAccessTimeUtc_ShouldSetCreationTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime lastAccessTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { LastAccessTime = lastAccessTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime newUtcTime = DateTime.UtcNow;
            fileInfo.LastAccessTimeUtc = newUtcTime;

            // Assert
            Assert.Equal(newUtcTime, fileInfo.LastAccessTimeUtc);
        }

        [Fact]
        public void MockFileInfo_LastWriteTimeUtc_ShouldReturnLastWriteTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime lastWriteTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { LastWriteTime = lastWriteTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime result = fileInfo.LastWriteTimeUtc;

            // Assert
            Assert.Equal(lastWriteTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFileInfo_LastWriteTimeUtc_ShouldSetLastWriteTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            DateTime lastWriteTime = DateTime.Now.AddHours(-4);
            MockFileData fileData = new MockFileData("Demo text content") { LastWriteTime = lastWriteTime };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            DateTime newUtcTime = DateTime.UtcNow;
            fileInfo.LastWriteTime = newUtcTime;

            // Assert
            Assert.Equal(newUtcTime, fileInfo.LastWriteTime);
        }

        [Fact]
        public void MockFileInfo_GetExtension_ShouldReturnExtension()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            // Act
            string result = fileInfo.Extension;

            // Assert
            Assert.Equal(".txt", result);
        }

        [Fact]
        public void MockFileInfo_GetExtensionWithoutExtension_ShouldReturnEmptyString()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            MockFileInfo fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a"));

            // Act
            string result = fileInfo.Extension;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void MockFileInfo_GetDirectoryName_ShouldReturnCompleteDirectoryPath()
        {
            // Arrange
            MockFileInfo fileInfo = new MockFileInfo(new MockFileSystem(), XFS.Path(@"c:\temp\level1\level2\file.txt"));

            // Act
            string result = fileInfo.DirectoryName;

            Assert.Equal(XFS.Path(@"c:\temp\level1\level2"), result);
        }

        [Fact]
        public void MockFileInfo_GetDirectory_ShouldReturnDirectoryInfoWithCorrectPath()
        {
            // Arrange
            MockFileInfo fileInfo = new MockFileInfo(new MockFileSystem(), XFS.Path(@"c:\temp\level1\level2\file.txt"));

            // Act
            DirectoryInfoBase result = fileInfo.Directory;

            Assert.Equal(XFS.Path(@"c:\temp\level1\level2"), result.FullName);
        }

        [Fact]
        public void MockFileInfo_OpenRead_ShouldReturnByteContentOfFile()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(new byte[] { 1, 2 }));
            FileInfoBase fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));

            // Act
            byte[] result = new byte[2];
            using (Stream stream = fileInfo.OpenRead())
            {
                stream.Read(result, 0, 2);
            }

            Assert.Equal(new byte[] { 1, 2 }, result);
        }

        [Fact]
        public void MockFileInfo_OpenText_ShouldReturnStringContentOfFile()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(@"line 1\r\nline 2"));
            FileInfoBase fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));

            // Act
            string result;
            using (StreamReader streamReader = fileInfo.OpenText())
            {
                result = streamReader.ReadToEnd();
            }

            Assert.Equal(@"line 1\r\nline 2", result);
        }

        [Fact]
        public void MockFileInfo_MoveTo_ShouldUpdateFileInfoDirectoryAndFullName()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(@"line 1\r\nline 2"));
            FileInfoBase fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));

            // Act
            string destinationFolder = XFS.Path(@"c:\temp2");
            string destination = XFS.Path(destinationFolder + @"\file.txt");
            fileSystem.AddDirectory(destination);
            fileInfo.MoveTo(destination);

            Assert.Equal(fileInfo.DirectoryName, destinationFolder);
            Assert.Equal(fileInfo.FullName, destination);
        }
    }
}