using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;
using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileWriteAllLinesTests
    {
        public static readonly string Path = XFS.Path(@"c:\something\demo.txt");

        // ReSharper disable once UnusedMember.Local
        public static IEnumerable ForDifferentEncoding
        {
            get
            {
                MockFileSystem fileSystem = new MockFileSystem();
                List<string> fileContentEnumerable = new List<string> { "first line", "second line", "third line", "fourth and last line" };
                string[] fileContentArray = fileContentEnumerable.ToArray();
                Action writeEnumberable = () => fileSystem.File.WriteAllLines(Path, fileContentEnumerable);
                Action writeEnumberableUtf32 = () => fileSystem.File.WriteAllLines(Path, fileContentEnumerable, Encoding.UTF32);
                Action writeArray = () => fileSystem.File.WriteAllLines(Path, fileContentArray);
                Action writeArrayUtf32 = () => fileSystem.File.WriteAllLines(Path, fileContentArray, Encoding.UTF32);
                string expectedContent = $"first line{Environment.NewLine}second line{Environment.NewLine}third line{Environment.NewLine}fourth and last line{Environment.NewLine}";

                // IEnumerable
                yield return new object[] { fileSystem, writeEnumberable, expectedContent };
                yield return new object[] { fileSystem, writeEnumberableUtf32, expectedContent };

                // string[]
                yield return new object[] { fileSystem, writeArray, expectedContent };
                yield return new object[] { fileSystem, writeArrayUtf32, expectedContent };
            }
        }

        public static IEnumerable ForIllegalPath
        {
            get
            {
                const string ILLEGAL_PATH = "<<<";
                return GetCasesForArgumentChecking(ILLEGAL_PATH);
            }
        }

        public static IEnumerable ForNullPath
        {
            get
            {
                const string ILLEGAL_PATH = null;
                return GetCasesForArgumentChecking(ILLEGAL_PATH);
            }
        }

        private static IEnumerable GetCasesForArgumentChecking(string path)
        {
            MockFileSystem fileSystem = new MockFileSystem();
            List<string> fileContentEnumerable = new List<string>();
            string[] fileContentArray = fileContentEnumerable.ToArray();
            Action writeEnumberable = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable);
            Action writeEnumberableUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable, Encoding.UTF32);
            Action writeArray = () => fileSystem.File.WriteAllLines(path, fileContentArray);
            Action writeArrayUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentArray, Encoding.UTF32);

            // IEnumerable
            yield return new object[] { writeEnumberable };
            yield return new object[] { writeEnumberableUtf32 };

            // string[]
            yield return new object[] { writeArray };
            yield return new object[] { writeArrayUtf32 };
        }

        public static IEnumerable ForNullEncoding
        {
            get
            {
                MockFileSystem fileSystem = new MockFileSystem();
                List<string> fileContentEnumerable = new List<string>();
                string[] fileContentArray = fileContentEnumerable.ToArray();
                Action writeEnumberableNull = () => fileSystem.File.WriteAllLines(Path, fileContentEnumerable, null);
                Action writeArrayNull = () => fileSystem.File.WriteAllLines(Path, fileContentArray, null);

                // IEnumerable
                yield return new object[] { writeEnumberableNull };

                // string[]
                yield return new object[] { writeArrayNull };
            }
        }

        public static IEnumerable ForPathIsDirectory
        {
            get
            {
                MockFileSystem fileSystem = new MockFileSystem();
                string path = XFS.Path(@"c:\something");
                fileSystem.Directory.CreateDirectory(path);
                List<string> fileContentEnumerable = new List<string>();
                string[] fileContentArray = fileContentEnumerable.ToArray();
                Action writeEnumberable = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable);
                Action writeEnumberableUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable, Encoding.UTF32);
                Action writeArray = () => fileSystem.File.WriteAllLines(path, fileContentArray);
                Action writeArrayUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentArray, Encoding.UTF32);

                // IEnumerable
                yield return new object[] { writeEnumberable, path };
                yield return new object[] { writeEnumberableUtf32, path };

                // string[]
                yield return new object[] { writeArray, path };
                yield return new object[] { writeArrayUtf32, path };
            }
        }

        public static IEnumerable ForFileIsReadOnly
        {
            get
            {
                MockFileSystem fileSystem = new MockFileSystem();
                string path = XFS.Path(@"c:\something\file.txt");
                MockFileData mockFileData = new MockFileData(string.Empty) {Attributes = FileAttributes.ReadOnly};
                fileSystem.AddFile(path, mockFileData);
                List<string> fileContentEnumerable = new List<string>();
                string[] fileContentArray = fileContentEnumerable.ToArray();
                Action writeEnumberable = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable);
                Action writeEnumberableUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable, Encoding.UTF32);
                Action writeArray = () => fileSystem.File.WriteAllLines(path, fileContentArray);
                Action writeArrayUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentArray, Encoding.UTF32);

                // IEnumerable
                yield return new object[] { writeEnumberable, path };
                yield return new object[] { writeEnumberableUtf32, path };

                // string[]
                yield return new object[] { writeArray, path };
                yield return new object[] { writeArrayUtf32, path };
            }
        }

        public static IEnumerable ForContentsIsNull
        {
            get
            {
                MockFileSystem fileSystem = new MockFileSystem();
                string path = XFS.Path(@"c:\something\file.txt");
                MockFileData mockFileData = new MockFileData(string.Empty) {Attributes = FileAttributes.ReadOnly};
                fileSystem.AddFile(path, mockFileData);
                List<string> fileContentEnumerable = null;
                string[] fileContentArray = null;

                // ReSharper disable ExpressionIsAlwaysNull
                Action writeEnumberable = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable);
                Action writeEnumberableUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentEnumerable, Encoding.UTF32);
                Action writeArray = () => fileSystem.File.WriteAllLines(path, fileContentArray);
                Action writeArrayUtf32 = () => fileSystem.File.WriteAllLines(path, fileContentArray, Encoding.UTF32);
                // ReSharper restore ExpressionIsAlwaysNull

                // IEnumerable
                yield return new object[] { writeEnumberable };
                yield return new object[] { writeEnumberableUtf32 };

                // string[]
                yield return new object[] { writeArray };
                yield return new object[] { writeArrayUtf32 };
            }
        }

        [Theory]
        [MemberData(nameof(ForDifferentEncoding))]
        public void MockFile_WriteAllLinesGeneric_ShouldWriteTheCorrectContent(IMockFileDataAccessor fileSystem, Action action, string expectedContent)
        {
            // Arrange
            // is done in the test case source

            // Act
            action();

            // Assert
            string actualContent = fileSystem.GetFile(Path).TextContents;
            Assert.Equal(actualContent, expectedContent);
        }

        [Theory]
        [MemberData(nameof(ForNullPath))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowAnArgumentNullExceptionIfPathIsNull(Action action)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case source

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Value cannot be null.", exception.Message);
            Assert.StartsWith("path", exception.ParamName);
        }

        [Theory]
        [MemberData(nameof(ForNullEncoding))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowAnArgumentNullExceptionIfEncodingIsNull(Action action)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case source

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Value cannot be null.", exception.Message);
            Assert.StartsWith("encoding", exception.ParamName);
        }

        [Theory]
        [MemberData(nameof(ForIllegalPath))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowAnArgumentExceptionIfPathContainsIllegalCharacters(Action action)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case source

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal(exception.Message, "Illegal characters in path.");
        }

        [Theory]
        [MemberData(nameof(ForPathIsDirectory))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowAnUnauthorizedAccessExceptionIfPathIsOneDirectory(Action action, string path)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case source

            // Assert
            UnauthorizedAccessException exception = Assert.Throws<UnauthorizedAccessException>(action);
            string expectedMessage = $"Access to the path '{path}' is denied.";
            Assert.Equal(exception.Message, expectedMessage);
        }

        [Theory]
        [MemberData(nameof(ForFileIsReadOnly))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowOneUnauthorizedAccessExceptionIfFileIsReadOnly(Action action, string path)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case source

            // Assert
            UnauthorizedAccessException exception = Assert.Throws<UnauthorizedAccessException>(action);
            string expectedMessage = $"Access to the path '{path}' is denied.";
            Assert.Equal(exception.Message, expectedMessage);
        }

        [Theory]
        [MemberData(nameof(ForContentsIsNull))]
        public void MockFile_WriteAllLinesGeneric_ShouldThrowAnArgumentNullExceptionIfContentsIsNull(Action action)
        {
            // Arrange
            // is done in the test case source

            // Act
            // is done in the test case sourceForContentsIsNull

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Value cannot be null.", exception.Message);
            Assert.Equal(exception.ParamName, "contents");
        }
    }
}
