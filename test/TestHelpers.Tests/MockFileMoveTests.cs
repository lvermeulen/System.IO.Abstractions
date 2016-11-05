using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileMoveTests {
        [Fact]
        public void MockFile_Move_ShouldMoveFileWithinMemoryFileSystem()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string sourceFileContent = "this is some content";
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFilePath, new MockFileData(sourceFileContent)},
                {XFS.Path(@"c:\somethingelse\dummy.txt"), new MockFileData(new byte[] {0})}
            });

            string destFilePath = XFS.Path(@"c:\somethingelse\demo1.txt");

            fileSystem.File.Move(sourceFilePath, destFilePath);

            Assert.True(fileSystem.FileExists(destFilePath));
            Assert.Equal(fileSystem.GetFile(destFilePath).TextContents, sourceFileContent);
            Assert.False(fileSystem.FileExists(sourceFilePath));
        }

        [Fact]
        public void MockFile_Move_ShouldThrowIOExceptionWhenTargetAlreadyExists()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string sourceFileContent = "this is some content";
            string destFilePath = XFS.Path(@"c:\somethingelse\demo1.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFilePath, new MockFileData(sourceFileContent)},
                {destFilePath, new MockFileData(sourceFileContent)}
            });

            IOException exception = Assert.Throws<IOException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

            Assert.Equal(exception.Message, "A file can not be created if it already exists.");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentNullExceptionWhenSourceIsNull_Message()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Move(null, destFilePath));

            Assert.StartsWith("File name cannot be null.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentNullExceptionWhenSourceIsNull_ParamName() {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Move(null, destFilePath));

            Assert.Equal(exception.ParamName, "sourceFileName");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowNotSupportedExceptionWhenSourceFileNameContainsInvalidChars_Message()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.True(true, "Path.GetInvalidChars() does not return anything on Mono");
            }

            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            foreach (char invalidChar in fileSystem.Path.GetInvalidFileNameChars().Where(x => x != fileSystem.Path.DirectorySeparatorChar))
            {
                string sourceFilePath = XFS.Path(@"c:\something\demo.txt") + invalidChar;

                ArgumentException exception =
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Move_ShouldThrowNotSupportedExceptionWhenSourcePathContainsInvalidChars_Message()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.True(true, "Path.GetInvalidChars() does not return anything on Mono");
            }

            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            foreach (char invalidChar in fileSystem.Path.GetInvalidPathChars())
            {
                string sourceFilePath = XFS.Path(@"c:\some" + invalidChar + @"thing\demo.txt");

                ArgumentException exception =
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Move_ShouldThrowNotSupportedExceptionWhenTargetPathContainsInvalidChars_Message()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.True(true, "Path.GetInvalidChars() does not return anything on Mono");
            }

            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            foreach (char invalidChar in fileSystem.Path.GetInvalidPathChars())
            {
                string destFilePath = XFS.Path(@"c:\some" + invalidChar + @"thing\demo.txt");

                ArgumentException exception =
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Move_ShouldThrowNotSupportedExceptionWhenTargetFileNameContainsInvalidChars_Message()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.True(true, "Path.GetInvalidChars() does not return anything on Mono");
            }

            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            foreach (char invalidChar in fileSystem.Path.GetInvalidFileNameChars().Where(x => x != fileSystem.Path.DirectorySeparatorChar))
            {
                string destFilePath = XFS.Path(@"c:\something\demo.txt") + invalidChar;

                ArgumentException exception =
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenSourceIsEmpty_Message()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(string.Empty, destFilePath));

            Assert.StartsWith("Empty file name is not legal.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenSourceIsEmpty_ParamName() {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(string.Empty, destFilePath));

            Assert.Equal(exception.ParamName, "sourceFileName");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenSourceIsStringOfBlanks()
        {
            string sourceFilePath = "   ";
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentNullExceptionWhenTargetIsNull_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Move(sourceFilePath, null));

            Assert.StartsWith("File name cannot be null.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentNullExceptionWhenTargetIsNull_ParamName() {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Move(sourceFilePath, null));

            Assert.Equal(exception.ParamName, "destFileName");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenTargetIsStringOfBlanks()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string destFilePath = "   ";
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenTargetIsEmpty_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, string.Empty));

            Assert.StartsWith("Empty file name is not legal.", exception.Message);
        }

        [Fact]
        public void MockFile_Move_ShouldThrowArgumentExceptionWhenTargetIsEmpty_ParamName() {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Move(sourceFilePath, string.Empty));

            Assert.Equal(exception.ParamName, "destFileName");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowFileNotFoundExceptionWhenSourceDoesNotExist_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string destFilePath = XFS.Path(@"c:\something\demo1.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

            Assert.Equal(exception.Message, $"The file \"{XFS.Path("c:\\something\\demo.txt")}\" could not be found.");
        }

        [Fact]
        public void MockFile_Move_ShouldThrowFileNotFoundExceptionWhenSourceDoesNotExist_FileName() {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string destFilePath = XFS.Path(@"c:\something\demo1.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));

            Assert.Equal(exception.FileName, XFS.Path(@"c:\something\demo.txt"));
        }

        [Fact]
        public void MockFile_Move_ShouldThrowDirectoryNotFoundExceptionWhenSourcePathDoesNotExist_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string destFilePath = XFS.Path(@"c:\somethingelse\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFilePath, new MockFileData(new byte[] {0})}
            });

            //var exists = fileSystem.Directory.Exists(XFS.Path(@"c:\something"));
            //exists = fileSystem.Directory.Exists(XFS.Path(@"c:\something22"));

            DirectoryNotFoundException exception = Assert.Throws<DirectoryNotFoundException>(() => fileSystem.File.Move(sourceFilePath, destFilePath));
            //Message = "Could not find a part of the path."
            Assert.Equal(exception.Message, XFS.Path(@"Could not find a part of the path."));
        }
    }
}