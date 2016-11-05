using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileCopyTests
    {
        [Fact]
        public void MockFile_Copy_ShouldOverwriteFileWhenOverwriteFlagIsTrue()
        {
            string sourceFileName = XFS.Path(@"c:\source\demo.txt");
            MockFileData sourceContents = new MockFileData("Source content");
            string destFileName = XFS.Path(@"c:\destination\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFileName, sourceContents},
                {destFileName, new MockFileData("Destination content")}
            });

            fileSystem.File.Copy(sourceFileName, destFileName, true);

            MockFileData copyResult = fileSystem.GetFile(destFileName);
            Assert.Equal(copyResult.Contents, sourceContents.Contents);
        }

        [Fact]
        public void MockFile_Copy_ShouldCreateFileAtNewDestination()
        {
            string sourceFileName = XFS.Path(@"c:\source\demo.txt");
            MockFileData sourceContents = new MockFileData("Source content");
            string destFileName = XFS.Path(@"c:\source\demo_copy.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFileName, sourceContents}
            });

            fileSystem.File.Copy(sourceFileName, destFileName, false);

            MockFileData copyResult = fileSystem.GetFile(destFileName);
            Assert.Equal(copyResult.Contents, sourceContents.Contents);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowExceptionWhenFileExistsAtDestination()
        {
            string sourceFileName = XFS.Path(@"c:\source\demo.txt");
            MockFileData sourceContents = new MockFileData("Source content");
            string destFileName = XFS.Path(@"c:\destination\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFileName, sourceContents},
                {destFileName, new MockFileData("Destination content")}
            });

            Assert.Throws<IOException>(() => fileSystem.File.Copy(sourceFileName, destFileName));
        }

        [Theory]
        [InlineData(@"c:\source\demo.txt", @"c:\source\doesnotexist\demo.txt")]
        [InlineData(@"c:\source\demo.txt", @"c:\doesnotexist\demo.txt")]
        public void MockFile_Copy_ShouldThrowExceptionWhenFolderInDestinationDoesNotExist(string sourceFilePath, string destFilePath)
        {
            string sourceFileName = XFS.Path(sourceFilePath);
            string destFileName = XFS.Path(destFilePath);
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {sourceFileName, MockFileData.NullObject}
            });

            Assert.Throws<DirectoryNotFoundException>(() => fileSystem.File.Copy(sourceFileName, destFileName));
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentNullExceptionWhenSourceIsNull_Message()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Copy(null, destFilePath));

            Assert.StartsWith("File name cannot be null.", exception.Message);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentNullExceptionWhenSourceIsNull_ParamName()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Copy(null, destFilePath));

            Assert.Equal(exception.ParamName, "sourceFileName");
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowNotSupportedExceptionWhenSourceFileNameContainsInvalidChars_Message()
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
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowNotSupportedExceptionWhenSourcePathContainsInvalidChars_Message()
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
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowNotSupportedExceptionWhenTargetPathContainsInvalidChars_Message()
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
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowNotSupportedExceptionWhenTargetFileNameContainsInvalidChars_Message()
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
                    Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

                Assert.Equal(exception.Message, "Illegal characters in path.");
            }
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentExceptionWhenSourceIsEmpty_Message()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(string.Empty, destFilePath));

            Assert.StartsWith("Empty file name is not legal.", exception.Message);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentExceptionWhenSourceIsEmpty_ParamName()
        {
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(string.Empty, destFilePath));

            Assert.Equal(exception.ParamName, "sourceFileName");
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentExceptionWhenSourceIsStringOfBlanks()
        {
            string sourceFilePath = "   ";
            string destFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentNullExceptionWhenTargetIsNull_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Copy(sourceFilePath, null));

            Assert.StartsWith("File name cannot be null.", exception.Message);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentNullExceptionWhenTargetIsNull_ParamName()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => fileSystem.File.Copy(sourceFilePath, null));

            Assert.Equal(exception.ParamName, "destFileName");
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentExceptionWhenTargetIsStringOfBlanks()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            string destFilePath = "   ";
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, destFilePath));

            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }

        [Fact]
        public void MockFile_Copy_ShouldThrowArgumentExceptionWhenTargetIsEmpty_Message()
        {
            string sourceFilePath = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => fileSystem.File.Copy(sourceFilePath, string.Empty));

            Assert.StartsWith("Empty file name is not legal.", exception.Message);
        }
    }
}