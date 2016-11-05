using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileTests
    {
        [Fact]
        public void MockFile_Constructor_ShouldThrowArgumentNullExceptionIfMockFileDataAccessorIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new MockFile(null));
        }

        [Fact]
        public void MockFile_GetSetCreationTime_ShouldPersist()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime creationTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetCreationTime(path, creationTime);
            DateTime result = file.GetCreationTime(path);

            // Assert
            Assert.Equal(creationTime, result);
        }

        [Fact]
        public void MockFile_SetCreationTimeUtc_ShouldAffectCreationTime()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime creationTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetCreationTimeUtc(path, creationTime.ToUniversalTime());
            DateTime result = file.GetCreationTime(path);

            // Assert
            Assert.Equal(creationTime, result);
        }

        [Fact]
        public void MockFile_SetCreationTime_ShouldAffectCreationTimeUtc()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime creationTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetCreationTime(path, creationTime);
            DateTime result = file.GetCreationTimeUtc(path);

            // Assert
            Assert.Equal(creationTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFile_GetSetLastAccessTime_ShouldPersist()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastAccessTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastAccessTime(path, lastAccessTime);
            DateTime result = file.GetLastAccessTime(path);

            // Assert
            Assert.Equal(lastAccessTime, result);
        }

        [Fact]
        public void MockFile_SetLastAccessTimeUtc_ShouldAffectLastAccessTime()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastAccessTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
            DateTime result = file.GetLastAccessTime(path);

            // Assert
            Assert.Equal(lastAccessTime, result);
        }

        [Fact]
        public void MockFile_SetLastAccessTime_ShouldAffectLastAccessTimeUtc()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastAccessTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastAccessTime(path, lastAccessTime);
            DateTime result = file.GetLastAccessTimeUtc(path);

            // Assert
            Assert.Equal(lastAccessTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFile_GetSetLastWriteTime_ShouldPersist()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastWriteTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastWriteTime(path, lastWriteTime);
            DateTime result = file.GetLastWriteTime(path);

            // Assert
            Assert.Equal(lastWriteTime, result);
        }

        static void ExecuteDefaultValueTest(Func<MockFile, string, DateTime> getDateValue)
        {
            DateTime expected = new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();
            MockFile file = new MockFile(fileSystem);

            DateTime actual = getDateValue(file, path);

            Assert.Equal(expected, actual.ToUniversalTime());
        }

        [Fact]
        public void MockFile_GetLastWriteTimeOfNonExistantFile_ShouldReturnDefaultValue()
        {
            ExecuteDefaultValueTest((f, p) => f.GetLastWriteTime(p));
        }

        [Fact]
        public void MockFile_GetLastWriteTimeUtcOfNonExistantFile_ShouldReturnDefaultValue() {
            ExecuteDefaultValueTest((f, p) => f.GetLastWriteTimeUtc(p));
        }

        [Fact]
        public void MockFile_GetLastAccessTimeUtcOfNonExistantFile_ShouldReturnDefaultValue() {
            ExecuteDefaultValueTest((f, p) => f.GetLastAccessTimeUtc(p));
        }

        [Fact]
        public void MockFile_GetLastAccessTimeOfNonExistantFile_ShouldReturnDefaultValue() {
            ExecuteDefaultValueTest((f, p) => f.GetLastAccessTime(p));
        }

        [Fact]
        public void MockFile_GetAttributeOfNonExistantFileButParentDirectoryExists_ShouldThrowOneFileNotFoundException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\something"));

            // Act

            // Assert
            Assert.Throws<FileNotFoundException>(() => fileSystem.File.GetAttributes(XFS.Path(@"c:\something\demo.txt")));
        }

        [Fact]
        public void MockFile_GetAttributeOfNonExistantFile_ShouldThrowOneDirectoryNotFoundException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() => fileSystem.File.GetAttributes(XFS.Path(@"c:\something\demo.txt")));
        }

        [Fact]
        public void MockFile_GetAttributeOfExistingFile_ShouldReturnCorrectValue()
        {
            MockFileData filedata = new MockFileData("test")
            {
                Attributes = FileAttributes.Hidden
            };
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"),  filedata }
            });

            FileAttributes attributes = fileSystem.File.GetAttributes(XFS.Path(@"c:\something\demo.txt"));
            Assert.Equal(FileAttributes.Hidden, attributes);
        }

        [Fact]
        public void MockFile_GetAttributeOfExistingUncDirectory_ShouldReturnCorrectValue()
        {
            MockFileData filedata = new MockFileData("test");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"\\share\folder\demo.txt"), filedata }
            });

            FileAttributes attributes = fileSystem.File.GetAttributes(XFS.Path(@"\\share\folder"));
            Assert.Equal(attributes, FileAttributes.Directory);
        }

        [Fact]
        public void MockFile_GetAttributeWithEmptyParameter_ShouldThrowOneArgumentException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.GetAttributes(string.Empty);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }

        [Fact]
        public void MockFile_GetAttributeWithIllegalParameter_ShouldThrowOneArgumentException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.File.GetAttributes(string.Empty);

            // Assert
            // Note: The actual type of the exception differs from the documentation.
            //       According to the documentation it should be of type NotSupportedException.
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockFile_GetCreationTimeOfNonExistantFile_ShouldReturnDefaultValue() {
            ExecuteDefaultValueTest((f, p) => f.GetCreationTime(p));
        }

        [Fact]
        public void MockFile_GetCreationTimeUtcOfNonExistantFile_ShouldReturnDefaultValue() {
            ExecuteDefaultValueTest((f, p) => f.GetCreationTimeUtc(p));
        }

        [Fact]
        public void MockFile_SetLastWriteTimeUtc_ShouldAffectLastWriteTime()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastWriteTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
            DateTime result = file.GetLastWriteTime(path);

            // Assert
            Assert.Equal(lastWriteTime, result);
        }

        [Fact]
        public void MockFile_SetLastWriteTime_ShouldAffectLastWriteTimeUtc()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });
            MockFile file = new MockFile(fileSystem);

            // Act
            DateTime lastWriteTime = new DateTime(2010, 6, 4, 13, 26, 42);
            file.SetLastWriteTime(path, lastWriteTime);
            DateTime result = file.GetLastWriteTimeUtc(path);

            // Assert
            Assert.Equal(lastWriteTime.ToUniversalTime(), result);
        }

        [Fact]
        public void MockFile_ReadAllBytes_ShouldReturnOriginalByteData()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\something\other.gif"), new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            byte[] result = file.ReadAllBytes(XFS.Path(@"c:\something\other.gif"));

            // Assert
            Assert.Equal(
                new byte[] { 0x21, 0x58, 0x3f, 0xa9 },
                result);
        }

        [Fact]
        public void MockFile_ReadAllText_ShouldReturnOriginalTextData()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\something\other.gif"), new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            string result = file.ReadAllText(XFS.Path(@"c:\something\demo.txt"));

            // Assert
            Assert.Equal(
                "Demo text content",
                result);
        }

        [Fact]
        public void MockFile_ReadAllText_ShouldReturnOriginalDataWithCustomEncoding()
        {
            // Arrange
            string text = "Hello there!";
            byte[] encodedText = Encoding.BigEndianUnicode.GetBytes(text);
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"), new MockFileData(encodedText) }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            string result = file.ReadAllText(XFS.Path(@"c:\something\demo.txt"), Encoding.BigEndianUnicode);

            // Assert
            Assert.Equal(text, result);
        }

        public static IEnumerable<Encoding> GetEncodingsForReadAllText()
        {
            // little endian
            yield return new UTF32Encoding(false, true, true);

            // big endian
            yield return new UTF32Encoding(true, true, true);
            yield return new UTF8Encoding(true, true);

            yield return new ASCIIEncoding();
        }

        public static IEnumerable<object[]> GetEncodingsForReadAllTextObjects()
        {
            foreach (var item in GetEncodingsForReadAllText())
            {
                yield return new object[] { item };
            }
        }

        [Theory]
        [MemberData(nameof(GetEncodingsForReadAllTextObjects))]
        public void MockFile_ReadAllText_ShouldReturnTheOriginalContentWhenTheFileContainsDifferentEncodings(Encoding encoding)
        {
            // Arrange
            string text = "Hello there!";
            byte[] encodedText = encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray();
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { path, new MockFileData(encodedText) }
                });

            // Act
            string actualText = fileSystem.File.ReadAllText(path);

            // Assert
            Assert.Equal(text, actualText);
        }

        [Fact]
        public void MockFile_ReadAllBytes_ShouldReturnDataSavedByWriteAllBytes()
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
                fileSystem.File.ReadAllBytes(path));
        }

        [Fact]
        public void MockFile_OpenWrite_ShouldCreateNewFiles() {
            string filePath = XFS.Path(@"c:\something\demo.txt");
            string fileContent = "this is some content";
            MockFileSystem fileSystem = new MockFileSystem();

            byte[] bytes = new UTF8Encoding(true).GetBytes(fileContent);
            Stream stream = fileSystem.File.OpenWrite(filePath);
            stream.Write(bytes, 0, bytes.Length);
            stream.Dispose();

            Assert.True(fileSystem.FileExists(filePath));
            Assert.Equal(fileSystem.GetFile(filePath).TextContents, fileContent);
        }

        [Fact]
        public void MockFile_OpenWrite_ShouldOverwriteExistingFiles()
        {
            string filePath = XFS.Path(@"c:\something\demo.txt");
            string startFileContent = "this is some content";
            string endFileContent = "this is some other content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath, new MockFileData(startFileContent)}
            });

            byte[] bytes = new UTF8Encoding(true).GetBytes(endFileContent);
            Stream stream = fileSystem.File.OpenWrite(filePath);
            stream.Write(bytes, 0, bytes.Length);
            stream.Dispose();

            Assert.True(fileSystem.FileExists(filePath));
            Assert.Equal(fileSystem.GetFile(filePath).TextContents, endFileContent);
        }

        [Fact]
        public void MockFile_Delete_ShouldRemoveFileFromFileSystem()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fullPath, new MockFileData("Demo text content") }
            });

            MockFile file = new MockFile(fileSystem);

            file.Delete(fullPath);

            Assert.False(fileSystem.FileExists(fullPath));
        }

        [Fact]
        public void MockFile_Delete_Should_RemoveFiles()
        {
            string filePath = XFS.Path(@"c:\something\demo.txt");
            string fileContent = "this is some content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { filePath, new MockFileData(fileContent) } });
            Assert.Equal(1, fileSystem.AllFiles.Count());
            fileSystem.File.Delete(filePath);
            Assert.Equal(0, fileSystem.AllFiles.Count());
        }

        [Fact]
        public void MockFile_Delete_No_File_Does_Nothing()
        {
            string filePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.File.Delete(filePath);
        }

        [Fact]
        public void MockFile_AppendText_AppendTextToanExistingFile()
        {
            string filepath = XFS.Path(@"c:\something\does\exist.txt");
            MockFileSystem filesystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { filepath, new MockFileData("I'm here. ") }
            });

            StreamWriter stream = filesystem.File.AppendText(filepath);

            stream.Write("Me too!");
            stream.Flush();
            stream.Dispose();

            MockFileData file = filesystem.GetFile(filepath);
            Assert.Equal(file.TextContents, "I'm here. Me too!");
        }

        [Fact]
        public void MockFile_AppendText_CreatesNewFileForAppendToNonExistingFile()
        {
            string filepath = XFS.Path(@"c:\something\doesnt\exist.txt");
            MockFileSystem filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            StreamWriter stream = filesystem.File.AppendText(filepath);

            stream.Write("New too!");
            stream.Flush();
            stream.Dispose();

            MockFileData file = filesystem.GetFile(filepath);
            Assert.Equal(file.TextContents, "New too!");
            Assert.True(filesystem.FileExists(filepath));
        }

        [Fact]
        public void Serializable_works()
        {
            //Arrange
            MockFileData data = new MockFileData("Text Contents");

            //Act
            var serializer = new XmlSerializer(data.GetType());
            Stream stream = new MemoryStream();
            serializer.Serialize(stream, data);

            //Assert
            Assert.True(true);
        }

        [Fact]
        public void Serializable_can_deserialize()
        {
            //Arrange
            string textContentStr = "Text Contents";

            //Act
            MockFileData data = new MockFileData(textContentStr);

            var serializer = new XmlSerializer(data.GetType());
            Stream stream = new MemoryStream();
            serializer.Serialize(stream, data);

            stream.Seek(0, SeekOrigin.Begin);

            MockFileData deserialized = (MockFileData)serializer.Deserialize(stream);

            //Assert
            Assert.Equal(deserialized.TextContents, textContentStr);
        }

        [Fact]
        public void MockFile_Encrypt_ShouldEncryptTheFile()
        {
            // Arrange
            const string CONTENT = "Demo text content";
            MockFileData fileData = new MockFileData(CONTENT);
            string filePath = XFS.Path(@"c:\a.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath, fileData }
            });

            // Act
            fileSystem.File.Encrypt(filePath);

            string newcontents;
            using (StreamReader newfile = fileSystem.File.OpenText(filePath))
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.NotEqual(CONTENT, newcontents);
        }

        [Fact]
        public void MockFile_Decrypt_ShouldDecryptTheFile()
        {
            // Arrange
            const string CONTENT = "Demo text content";
            MockFileData fileData = new MockFileData(CONTENT);
            string filePath = XFS.Path(@"c:\a.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath, fileData }
            });

            // Act
            fileSystem.File.Decrypt(filePath);

            string newcontents;
            using (StreamReader newfile = fileSystem.File.OpenText(filePath))
            {
                newcontents = newfile.ReadToEnd();
            }

            // Assert
            Assert.NotEqual(CONTENT, newcontents);
        }
    }
}
