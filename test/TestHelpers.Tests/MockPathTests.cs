using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockPathTests
    {
        private static readonly string s_testPath = XFS.Path("C:\\test\\test.bmp");

        [Fact]
        public void ChangeExtension_ExtensionNoPeriod_PeriodAdded()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.ChangeExtension(s_testPath, "doc");

            //Assert
            Assert.Equal(XFS.Path("C:\\test\\test.doc"), result);
        }

        [Fact]
        public void Combine_SentTwoPaths_Combines()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.Combine(XFS.Path("C:\\test"), "test.bmp");

            //Assert
            Assert.Equal(XFS.Path("C:\\test\\test.bmp"), result);
        }

        [Fact]
        public void Combine_SentThreePaths_Combines()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.Combine(XFS.Path("C:\\test"), "subdir1", "test.bmp");

            //Assert
            Assert.Equal(XFS.Path("C:\\test\\subdir1\\test.bmp"), result);
        }

        [Fact]
        public void Combine_SentFourPaths_Combines()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.Combine(XFS.Path("C:\\test"), "subdir1", "subdir2", "test.bmp");

            //Assert
            Assert.Equal(XFS.Path("C:\\test\\subdir1\\subdir2\\test.bmp"), result);
        }

        [Fact]
        public void Combine_SentFivePaths_Combines()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.Combine(XFS.Path("C:\\test"), "subdir1", "subdir2", "subdir3", "test.bmp");

            //Assert
            Assert.Equal(XFS.Path("C:\\test\\subdir1\\subdir2\\subdir3\\test.bmp"), result);
        }

        [Fact]
        public void GetDirectoryName_SentPath_ReturnsDirectory()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetDirectoryName(s_testPath);

            //Assert
            Assert.Equal(XFS.Path("C:\\test"), result);
        }

        [Fact]
        public void GetExtension_SendInPath_ReturnsExtension()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetExtension(s_testPath);

            //Assert
            Assert.Equal(".bmp", result);
        }

        [Fact]
        public void GetFileName_SendInPath_ReturnsFilename()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetFileName(s_testPath);

            //Assert
            Assert.Equal("test.bmp", result);
        }

        [Fact]
        public void GetFileNameWithoutExtension_SendInPath_ReturnsFileNameNoExt()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetFileNameWithoutExtension(s_testPath);

            //Assert
            Assert.Equal("test", result);
        }

        [Fact]
        public void GetFullPath_SendInPath_ReturnsFullPath()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetFullPath(s_testPath);

            //Assert
            Assert.Equal(s_testPath, result);
        }

        public static IEnumerable<string[]> GetFullPathRelativePathsCases
        {
            get
            {
                yield return new [] { XFS.Path(@"c:\a"), "b", XFS.Path(@"c:\a\b") };
                yield return new [] { XFS.Path(@"c:\a\b"), "c", XFS.Path(@"c:\a\b\c") };
                yield return new [] { XFS.Path(@"c:\a\b"), XFS.Path(@"c\"), XFS.Path(@"c:\a\b\c\") };
                yield return new [] { XFS.Path(@"c:\a\b"), XFS.Path(@"..\c"), XFS.Path(@"c:\a\c") };
                yield return new [] { XFS.Path(@"c:\a\b\c"), XFS.Path(@"..\c\..\"), XFS.Path(@"c:\a\b\") };
                yield return new [] { XFS.Path(@"c:\a\b\c"), XFS.Path(@"..\..\..\..\..\d"), XFS.Path(@"c:\d") };
                yield return new [] { XFS.Path(@"c:\a\b\c"), XFS.Path(@"..\..\..\..\..\d\"), XFS.Path(@"c:\d\") };
            }
        }

        [Theory]
        [MemberData(nameof(GetFullPathRelativePathsCases))]
        public void GetFullPath_RelativePaths_ShouldReturnTheAbsolutePathWithCurrentDirectory(string currentDir, string relativePath, string expectedResult)
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.Directory.SetCurrentDirectory(currentDir);
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            string actualResult = mockPath.GetFullPath(relativePath);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<string[]> GetFullPathRootedPathWithRelativeSegmentsCases
        {
            get
            {
                yield return new [] { XFS.Path(@"c:\a\b\..\c"), XFS.Path(@"c:\a\c") };
                yield return new [] { XFS.Path(@"c:\a\b\.\.\..\.\c"), XFS.Path(@"c:\a\c") };
                yield return new [] { XFS.Path(@"c:\a\b\.\c"), XFS.Path(@"c:\a\b\c") };
                yield return new [] { XFS.Path(@"c:\a\b\.\.\.\.\c"), XFS.Path(@"c:\a\b\c") };
                yield return new [] { XFS.Path(@"c:\a\..\..\c"), XFS.Path(@"c:\c") };
            }
        }

        [Theory]
        [MemberData(nameof(GetFullPathRootedPathWithRelativeSegmentsCases))]
        public void GetFullPath_RootedPathWithRelativeSegments_ShouldReturnAnRootedAbsolutePath(string rootedPath, string expectedResult)
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            string actualResult = mockPath.GetFullPath(rootedPath);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<string[]> GetFullPathAbsolutePathsCases
        {
            get
            {
                yield return new [] { XFS.Path(@"c:\a"), XFS.Path(@"/b"), XFS.Path(@"c:\b") };
                yield return new [] { XFS.Path(@"c:\a"), XFS.Path(@"/b\"), XFS.Path(@"c:\b\") };
                yield return new [] { XFS.Path(@"c:\a"), XFS.Path(@"\b"), XFS.Path(@"c:\b") };
                yield return new [] { XFS.Path(@"c:\a"), XFS.Path(@"\b\..\c"), XFS.Path(@"c:\c") };
                yield return new [] { XFS.Path(@"z:\a"), XFS.Path(@"\b\..\c"), XFS.Path(@"z:\c") };
                yield return new [] { XFS.Path(@"z:\a"), XFS.Path(@"\\computer\share\c"), XFS.Path(@"\\computer\share\c") };
                yield return new [] { XFS.Path(@"z:\a"), XFS.Path(@"\\computer\share\c\..\d"), XFS.Path(@"\\computer\share\d") };
                yield return new [] { XFS.Path(@"z:\a"), XFS.Path(@"\\computer\share\c\..\..\d"), XFS.Path(@"\\computer\share\d") };
            }
        }

        [Theory]
        [MemberData(nameof(GetFullPathAbsolutePathsCases))]
        public void GetFullPath_AbsolutePaths_ShouldReturnThePathWithTheRoot_Or_Unc(string currentDir, string absolutePath, string expectedResult)
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.Directory.SetCurrentDirectory(currentDir);
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            string actualResult = mockPath.GetFullPath(absolutePath);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetFullPath_InvalidUNCPaths_ShouldThrowArgumentException()
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            Action action = () => mockPath.GetFullPath(XFS.Path(@"\\shareZ"));

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetFullPath_NullValue_ShouldThrowArgumentNullException()
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            Action action = () => mockPath.GetFullPath(null);

            //Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void GetFullPath_EmptyValue_ShouldThrowArgumentException()
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            Action action = () => mockPath.GetFullPath(string.Empty);

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetFullPath_WithMultipleDirectorySeparators_ShouldReturnTheNormalizedForm()
        {
            //Arrange
            MockFileSystem mockFileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(mockFileSystem);

            //Act
            string actualFullPath =  mockPath.GetFullPath(XFS.Path(@"c:\foo\\//bar\file.dat"));

            //Assert
            Assert.Equal(XFS.Path(@"c:\foo\bar\file.dat"), actualFullPath);
        }

        [Fact]
        public void GetInvalidFileNameChars_Called_ReturnsChars()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            char[] result = mockPath.GetInvalidFileNameChars();

            //Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GetInvalidPathChars_Called_ReturnsChars()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            char[] result = mockPath.GetInvalidPathChars();

            //Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GetPathRoot_SendInPath_ReturnsRoot()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetPathRoot(s_testPath);

            //Assert
            Assert.Equal(XFS.Path("C:\\"), result);
        }

        [Fact]
        public void GetRandomFileName_Called_ReturnsStringLengthGreaterThanZero()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetRandomFileName();

            //Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GetTempFileName_Called_ReturnsStringLengthGreaterThanZero()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetTempFileName();

            //Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GetTempFileName_Called_CreatesEmptyFileInTempDirectory()
        {
            //Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            MockPath mockPath = new MockPath(fileSystem);

            //Act
            string result = mockPath.GetTempFileName();

            Assert.True(fileSystem.FileExists(result));
            Assert.Equal(0, fileSystem.FileInfo.FromFileName(result).Length);
        }

        [Fact]
        public void GetTempPath_Called_ReturnsStringLengthGreaterThanZero()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            string result = mockPath.GetTempPath();

            //Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void HasExtension_PathSentIn_DeterminesExtension()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            bool result = mockPath.HasExtension(s_testPath);

            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void IsPathRooted_PathSentIn_DeterminesPathExists()
        {
            //Arrange
            MockPath mockPath = new MockPath(new MockFileSystem());

            //Act
            bool result = mockPath.IsPathRooted(s_testPath);

            //Assert
            Assert.Equal(true, result);
        }
    }
}
