﻿using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockDirectoryTests
    {
        [Fact]
        public void MockDirectory_GetFiles_ShouldReturnAllFilesBelowPathWhenPatternIsWildcardAndSearchOptionIsAllDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[]
            {
                XFS.Path(@"c:\a\a.txt"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\c.txt"),
                XFS.Path(@"c:\a\a\a.txt"),
                XFS.Path(@"c:\a\a\b.txt"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\a"), "*", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        private MockFileSystem SetupFileSystem() => new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\c.gif"), new MockFileData("Demo text content") },
            });

        [Fact]
        public void MockDirectory_GetFiles_ShouldReturnFilesDirectlyBelowPathWhenPatternIsWildcardAndSearchOptionIsTopDirectoryOnly()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[]
            {
                XFS.Path(@"c:\a\a.txt"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\c.txt")
            };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\a"), "*", SearchOption.TopDirectoryOnly);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPattern()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[]
            {
                XFS.Path(@"c:\a.gif"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), "*.gif", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPatternWithThreeCharacterLongFileExtension_RepectingAllDirectorySearchOption()
        {
            // Arrange
            string additionalFilePath = XFS.Path(@"c:\a\a\c.gifx");
            MockFileSystem fileSystem = SetupFileSystem();
            fileSystem.AddFile(additionalFilePath, new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx.xyz"), new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx\xyz"), new MockFileData(string.Empty));
            string[] expected = new[]
                {
                    XFS.Path(@"c:\a.gif"),
                    XFS.Path(@"c:\a\b.gif"),
                    XFS.Path(@"c:\a\a\c.gif"),
                    additionalFilePath
                };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), "*.gif", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPatternWithThreeCharacterLongFileExtension_RepectingTopDirectorySearchOption()
        {
            // Arrange
            string additionalFilePath = XFS.Path(@"c:\a\c.gifx");
            MockFileSystem fileSystem = SetupFileSystem();
            fileSystem.AddFile(additionalFilePath, new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx.xyz"), new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx"), new MockFileData(string.Empty));
            string[] expected = new[]
                {
                    XFS.Path(@"c:\a\b.gif"),
                    additionalFilePath
                };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\a"), "*.gif", SearchOption.TopDirectoryOnly);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPatternOnlyIfTheFileExtensionIsThreeCharacterLong()
        {
            // Arrange
            string additionalFilePath = XFS.Path(@"c:\a\c.gi");
            MockFileSystem fileSystem = SetupFileSystem();
            fileSystem.AddFile(additionalFilePath, new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx.xyz"), new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gif"), new MockFileData(string.Empty));
            fileSystem.AddFile(XFS.Path(@"c:\a\a\c.gifx"), new MockFileData(string.Empty));
            string[] expected = new[]
                {
                    additionalFilePath
                };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\a"), "*.gi", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPatternWithDotsInFilenames()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.there.are.dots.in.this.filename.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\c.gif"), new MockFileData("Demo text content") },
            });
            string[] expected = new[]
            {
                XFS.Path(@"c:\a.there.are.dots.in.this.filename.gif"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), "*.gif", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_FilterShouldFindFilesWithSpecialChars()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.1#.pdf"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\b\b #1.txt"), new MockFileData("Demo text content") }
            });
            string[] expected = new[]
            {
                XFS.Path(@"c:\a.1#.pdf"),
                XFS.Path(@"c:\b\b #1.txt")
            };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), "*.*", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFilterByExtensionBasedSearchPatternAndSearchOptionTopDirectoryOnly()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[] { XFS.Path(@"c:\a.gif") };

            // Act
            string[] result = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), "*.gif", SearchOption.TopDirectoryOnly);

            // Assert
            Assert.Equal(result, expected);
        }

        private void ExecuteTimeAttributeTest(Action<IFileSystem, string, DateTime> setter, Func<IFileSystem, string, DateTime> getter)
        {
            string path = XFS.Path(@"c:\something\demo.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });

            // Act
            DateTime time = new DateTime(2010, 6, 4, 13, 26, 42);
            setter(fileSystem, path, time);
            DateTime result = getter(fileSystem, path);

            // Assert
            Assert.Equal(result, time);
        }

        [Fact]
        public void MockDirectory_GetCreationTime_ShouldReturnCreationTimeFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetCreationTime(p, d),
                (fs, p) => fs.Directory.GetCreationTime(p));
        }

        [Fact]
        public void MockDirectory_GetCreationTimeUtc_ShouldReturnCreationTimeUtcFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetCreationTimeUtc(p, d),
                (fs, p) => fs.Directory.GetCreationTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_GetLastAccessTime_ShouldReturnLastAccessTimeFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetLastAccessTime(p, d),
                (fs, p) => fs.Directory.GetLastAccessTime(p));
        }

        [Fact]
        public void MockDirectory_GetLastAccessTimeUtc_ShouldReturnLastAccessTimeUtcFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetLastAccessTimeUtc(p, d),
                (fs, p) => fs.Directory.GetLastAccessTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_GetLastWriteTime_ShouldReturnLastWriteTimeFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetLastWriteTime(p, d),
                (fs, p) => fs.Directory.GetLastWriteTime(p));
        }

        [Fact]
        public void MockDirectory_GetLastWriteTimeUtc_ShouldReturnLastWriteTimeUtcFromFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.File.SetLastWriteTimeUtc(p, d),
                (fs, p) => fs.Directory.GetLastWriteTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_SetCreationTime_ShouldSetCreationTimeOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetCreationTime(p, d),
                (fs, p) => fs.File.GetCreationTime(p));
        }

        [Fact]
        public void MockDirectory_SetCreationTimeUtc_ShouldSetCreationTimeUtcOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetCreationTimeUtc(p, d),
                (fs, p) => fs.File.GetCreationTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_SetLastAccessTime_ShouldSetLastAccessTimeOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetLastAccessTime(p, d),
                (fs, p) => fs.File.GetLastAccessTime(p));
        }

        [Fact]
        public void MockDirectory_SetLastAccessTimeUtc_ShouldSetLastAccessTimeUtcOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetLastAccessTimeUtc(p, d),
                (fs, p) => fs.File.GetLastAccessTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_SetLastWriteTime_ShouldSetLastWriteTimeOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetLastWriteTime(p, d),
                (fs, p) => fs.File.GetLastWriteTime(p));
        }

        [Fact]
        public void MockDirectory_SetLastWriteTimeUtc_ShouldSetLastWriteTimeUtcOnFile()
        {
            ExecuteTimeAttributeTest(
                (fs, p, d) => fs.Directory.SetLastWriteTimeUtc(p, d),
                (fs, p) => fs.File.GetLastWriteTimeUtc(p));
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnTrueForDirectoryDefinedInMemoryFileSystemWithoutTrailingSlash()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") }
            });

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\foo"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnTrueForDirectoryDefinedInMemoryFileSystemWithTrailingSlash()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") }
            });

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\foo\"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnFalseForDirectoryNotDefinedInMemoryFileSystemWithoutTrailingSlash()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") }
            });

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\baz"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnFalseForDirectoryNotDefinedInMemoryFileSystemWithTrailingSlash()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") }
            });

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\baz\"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnFalseForDirectoryNotDefinedInMemoryFileSystemWithSimilarFileName()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\baz.txt"), new MockFileData("Demo text content") }
            });

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\baz"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnTrueForDirectoryCreatedViaMocks()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content") }
            });
            fileSystem.Directory.CreateDirectory(XFS.Path(@"c:\bar"));

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\bar"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnTrueForFolderContainingFileAddedToMockFileSystem()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content"));

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\foo\"));

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(@"\\s")]
        [InlineData(@"<")]
        [InlineData("\t")]
        public void MockDirectory_Exists_ShouldReturnFalseForIllegalPath(string path)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            bool result = fileSystem.Directory.Exists(path);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockDirectory_Exists_ShouldReturnFalseForFiles()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\foo\bar.txt"), new MockFileData("Demo text content"));

            // Act
            bool result = fileSystem.Directory.Exists(XFS.Path(@"c:\foo\bar.txt"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MockDirectory_CreateDirectory_ShouldCreateFolderInMemoryFileSystem()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo.txt"), new MockFileData("Demo text content") }
            });

            // Act
            fileSystem.Directory.CreateDirectory(XFS.Path(@"c:\bar"));

            // Assert
            Assert.True(fileSystem.FileExists(XFS.Path(@"c:\bar\")));
            Assert.True(fileSystem.AllDirectories.Any(d => d == XFS.Path(@"c:\bar\")));
        }

        [Fact]
        public void MockDirectory_CreateDirectory_ShouldReturnDirectoryInfoBase()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\foo.txt"), new MockFileData("Demo text content") }
            });

            // Act
            DirectoryInfoBase result = fileSystem.Directory.CreateDirectory(XFS.Path(@"c:\bar"));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MockDirectory_CreateDirectory_ShouldWorkWithUNCPath()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            fileSystem.Directory.CreateDirectory(XFS.Path(@"\\server\share\path\to\create", () => false));

            // Assert
            Assert.True(fileSystem.Directory.Exists(XFS.Path(@"\\server\share\path\to\create\", () => false)));
        }

        [Fact]
        public void MockDirectory_CreateDirectory_ShouldFailIfTryingToCreateUNCPathOnlyServer()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            ArgumentException ex = Assert.Throws<ArgumentException>(() => fileSystem.Directory.CreateDirectory(XFS.Path(@"\\server", () => false)));

            // Assert
            Assert.StartsWith("The UNC path should be of the form \\\\server\\share.", ex.Message);
            Assert.Equal(ex.ParamName, "path");
        }

        [Fact]
        public void MockDirectory_CreateDirectory_ShouldSucceedIfTryingToCreateUNCPathShare()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            fileSystem.Directory.CreateDirectory(XFS.Path(@"\\server\share", () => false));

            // Assert
            Assert.True(fileSystem.Directory.Exists(XFS.Path(@"\\server\share\", () => false)));
        }

        [Fact]
        public void MockDirectory_Delete_ShouldDeleteDirectory()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\bar\foo.txt"), new MockFileData("Demo text content") }
            });

            // Act
            fileSystem.Directory.Delete(XFS.Path(@"c:\bar"), true);

            // Assert
            Assert.False(fileSystem.Directory.Exists(XFS.Path(@"c:\bar")));
        }

        [Fact]
        public void MockDirectory_Delete_ShouldDeleteDirectoryCaseInsensitively()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\bar\foo.txt"), new MockFileData("Demo text content") }
            });

            // Act
            fileSystem.Directory.Delete(XFS.Path(@"c:\BAR"), true);

            // Assert
            Assert.False(fileSystem.Directory.Exists(XFS.Path(@"c:\bar")));
        }

        [Fact]
        public void MockDirectory_Delete_ShouldThrowDirectoryNotFoundException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\bar\foo.txt"), new MockFileData("Demo text content") }
            });

            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(() => fileSystem.Directory.Delete(XFS.Path(@"c:\baz")));

            Assert.Equal(ex.Message, XFS.Path("c:\\baz\\") + " does not exist or could not be found.");
        }

        [Fact]
        public void MockDirectory_Delete_ShouldThrowIOException()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\bar\foo.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\bar\baz.txt"), new MockFileData("Demo text content") }
            });

            IOException ex = Assert.Throws<IOException>(() => fileSystem.Directory.Delete(XFS.Path(@"c:\bar")));

            Assert.Equal(ex.Message, "The directory specified by " + XFS.Path("c:\\bar\\") + " is read-only, or recursive is false and " + XFS.Path("c:\\bar\\") + " is not an empty directory.");
        }

        [Fact]
        public void MockDirectory_Delete_ShouldDeleteDirectoryRecursively()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\bar\foo.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\bar\bar2\foo.txt"), new MockFileData("Demo text content") }
            });

            // Act
            fileSystem.DirectoryInfo.FromDirectoryName(XFS.Path(@"c:\bar")).Delete(true);

            // Assert
            Assert.False(fileSystem.Directory.Exists(XFS.Path(@"c:\bar")));
            Assert.False(fileSystem.Directory.Exists(XFS.Path(@"c:\bar\bar2")));
        }

        [Fact]
        public void MockDirectory_GetFileSystemEntries_Returns_Files_And_Directories()
        {
            string testPath = XFS.Path(@"c:\foo\bar.txt");
            string testDir =  XFS.Path(@"c:\foo\bar\");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { testPath, new MockFileData("Demo text content") },
                { testDir,  new MockDirectoryData() }
            });

            IOrderedEnumerable<string> entries = fileSystem.Directory.GetFileSystemEntries(XFS.Path(@"c:\foo")).OrderBy(k => k);
            Assert.Equal(2, entries.Count());
            Assert.Equal(testDir, entries.Last());
            Assert.Equal(testPath, entries.First());
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldThrowArgumentNullException_IfPathParamIsNull()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            Action action = () => fileSystem.Directory.GetFiles(null);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldThrowDirectoryNotFoundException_IfPathDoesNotExists()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => fileSystem.Directory.GetFiles(XFS.Path(@"c:\Foo"), "*a.txt");

            // Assert
            Assert.Throws<DirectoryNotFoundException>(action);
        }

        [Fact]
        public void MockDirectory_GetFiles_Returns_Files()
        {
            string testPath = XFS.Path(@"c:\foo\bar.txt");
            string testDir = XFS.Path(@"c:\foo\bar\");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { testPath, new MockFileData("Demo text content") },
                { testDir,  new MockDirectoryData() }
            });

            IOrderedEnumerable<string> entries = fileSystem.Directory.GetFiles(XFS.Path(@"c:\foo")).OrderBy(k => k);
            Assert.Equal(1, entries.Count());
            Assert.Equal(testPath, entries.First());
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldThrowAnArgumentNullException_IfSearchPatternIsNull()
        {
            // Arrange
            string directoryPath = XFS.Path(@"c:\Foo");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(directoryPath);

            // Act
            Action action = () => fileSystem.Directory.GetFiles(directoryPath, null);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldThrowAnArgumentException_IfSearchPatternEndsWithTwoDots()
        {
            // Arrange
            string directoryPath = XFS.Path(@"c:\Foo");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(directoryPath);

            // Act
            Action action = () => fileSystem.Directory.GetFiles(directoryPath, "*a..");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        public static IEnumerable<string> GetSearchPatternForTwoDotsExceptions()
        {
            yield return @"a..\b";
            yield return @"a../b";
            yield return @"../";
            yield return @"..\";
            yield return @"aaa\vv..\";
        }

        public static IEnumerable<object[]> GetSearchPatternForTwoDotsExceptionsObjects()
        {
            foreach (string item in GetSearchPatternForTwoDotsExceptions())
            {
                yield return new object[] { item };
            }
        }

        [Theory]
        [MemberData(nameof(GetSearchPatternForTwoDotsExceptionsObjects))]
        public void MockDirectory_GetFiles_ShouldThrowAnArgumentException_IfSearchPatternContainsTwoDotsFollowedByOneDirectoryPathSep(string searchPattern)
        {
            // Arrange
            string directoryPath = XFS.Path(@"c:\Foo");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(directoryPath);

            // Act
            Action action = () => fileSystem.Directory.GetFiles(directoryPath, searchPattern);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockDirectory_GetFiles_ShouldFindFilesContainingTwoOrMoreDots()
        {
            // Arrange
            string testPath = XFS.Path(@"c:\foo..r\bar.txt");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { testPath, new MockFileData(string.Empty) }
                });

            // Act
            string[] actualResult = fileSystem.Directory.GetFiles(XFS.Path(@"c:\"), @"foo..r\*");

            // Assert
            Assert.Equal(actualResult, new [] { testPath });
        }

        [Theory]
        [InlineData(@"""")]
        [InlineData("aa\t")]
        public void MockDirectory_GetFiles_ShouldThrowAnArgumentException_IfSearchPatternHasIllegalCharacters(string searchPattern)
        {
            // Arrange
            string directoryPath = XFS.Path(@"c:\Foo");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(directoryPath);

            // Act
            Action action = () => fileSystem.Directory.GetFiles(directoryPath, searchPattern);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockDirectory_GetRoot_Returns_Root()
        {
            string testDir = XFS.Path(@"c:\foo\bar\");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { testDir,  new MockDirectoryData() }
            });

            Assert.Equal(XFS.Path("C:\\"), fileSystem.Directory.GetDirectoryRoot(XFS.Path(@"C:\foo\bar")));
        }

        [Fact]
        public void MockDirectory_GetLogicalDrives_Returns_LogicalDrives()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    {XFS.Path(@"c:\foo\bar\"), new MockDirectoryData()},
                    {XFS.Path(@"c:\foo\baz\"), new MockDirectoryData()},
                    {XFS.Path(@"d:\bash\"), new MockDirectoryData()},
                });

            string[] drives = fileSystem.Directory.GetLogicalDrives();

            if (XFS.IsUnixPlatform())
            {
                Assert.Equal(1, drives.Length);
                Assert.True(drives.Contains("/"));
            }
            else
            {
                Assert.Equal(2, drives.Length);
                Assert.True(drives.Contains("c:\\"));
                Assert.True(drives.Contains("d:\\"));
            }
        }

        [Fact]
        public void MockDirectory_GetDirectories_Returns_Child_Directories()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"A:\folder1\folder2\folder3\file.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"A:\folder1\folder4\file2.txt"), new MockFileData("Demo text content 2") },
            });

            string[] directories = fileSystem.Directory.GetDirectories(XFS.Path(@"A:\folder1")).ToArray();

            //Check that it does not returns itself
            Assert.False(directories.Contains(XFS.Path(@"A:\folder1\")));

            //Check that it correctly returns all child directories
            Assert.Equal(2, directories.Length);
            Assert.True(directories.Contains(XFS.Path(@"A:\folder1\folder2\")));
            Assert.True(directories.Contains(XFS.Path(@"A:\folder1\folder4\")));
        }

        [Fact]
        public void MockDirectory_GetDirectories_WithTopDirectories_ShouldOnlyReturnTopDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo.foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\.foo"));
            fileSystem.AddFile(XFS.Path(@"C:\Folder\.foo\bar"), new MockFileData(string.Empty));

            // Act
            string[] actualResult = fileSystem.Directory.GetDirectories(XFS.Path(@"c:\Folder\"), "*.foo");

            // Assert
            Assert.Equal(actualResult, new []{XFS.Path(@"C:\Folder\.foo\"), XFS.Path(@"C:\Folder\foo.foo\")});
        }

        [Fact]
        public void MockDirectory_GetDirectories_WithAllDirectories_ShouldReturnsAllMatchingSubFolders()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo.foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\.foo"));
            fileSystem.AddFile(XFS.Path(@"C:\Folder\.foo\bar"), new MockFileData(string.Empty));

            // Act
            string[] actualResult = fileSystem.Directory.GetDirectories(XFS.Path(@"c:\Folder\"), "*.foo", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(actualResult, new[] { XFS.Path(@"C:\Folder\.foo\"), XFS.Path(@"C:\Folder\foo.foo\"), XFS.Path(@"C:\Folder\.foo\.foo\") });
        }

        [Fact]
        public void MockDirectory_GetDirectories_ShouldThrowWhenPathIsNotMocked()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\c.gif"), new MockFileData("Demo text content") },
            });

            // Act
            Action action = () => fileSystem.Directory.GetDirectories(XFS.Path(@"c:\d"));

            // Assert
            Assert.Throws<DirectoryNotFoundException>(action);
        }

        [Fact]
        public void MockDirectory_EnumerateDirectories_Returns_Child_Directories()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"A:\folder1\folder2\folder3\file.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"A:\folder1\folder4\file2.txt"), new MockFileData("Demo text content 2") },
            });

            string[] directories = fileSystem.Directory.EnumerateDirectories(XFS.Path(@"A:\folder1")).ToArray();

            //Check that it does not returns itself
            Assert.False(directories.Contains(XFS.Path(@"A:\folder1\")));

            //Check that it correctly returns all child directories
            Assert.Equal(2, directories.Length);
            Assert.True(directories.Contains(XFS.Path(@"A:\folder1\folder2\")));
            Assert.True(directories.Contains(XFS.Path(@"A:\folder1\folder4\")));
        }

        [Fact]
        public void MockDirectory_EnumerateDirectories_WithTopDirectories_ShouldOnlyReturnTopDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo.foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\.foo"));
            fileSystem.AddFile(XFS.Path(@"C:\Folder\.foo\bar"), new MockFileData(string.Empty));

            // Act
            IEnumerable<string> actualResult = fileSystem.Directory.EnumerateDirectories(XFS.Path(@"c:\Folder\"), "*.foo");

            // Assert
            Assert.Equal(actualResult, new[] { XFS.Path(@"C:\Folder\.foo\"), XFS.Path(@"C:\Folder\foo.foo\") });
        }

        [Fact]
        public void MockDirectory_EnumerateDirectories_WithAllDirectories_ShouldReturnsAllMatchingSubFolders()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\foo.foo"));
            fileSystem.AddDirectory(XFS.Path(@"C:\Folder\.foo\.foo"));
            fileSystem.AddFile(XFS.Path(@"C:\Folder\.foo\bar"), new MockFileData(string.Empty));

            // Act
            IEnumerable<string> actualResult = fileSystem.Directory.EnumerateDirectories(XFS.Path(@"c:\Folder\"), "*.foo", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(actualResult, new[] { XFS.Path(@"C:\Folder\.foo\"), XFS.Path(@"C:\Folder\foo.foo\"), XFS.Path(@"C:\Folder\.foo\.foo\") });
        }

        [Fact]
        public void MockDirectory_EnumerateDirectories_ShouldThrowWhenPathIsNotMocked()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b.gif"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\c.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\b.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\a\c.gif"), new MockFileData("Demo text content") },
            });

            // Act
            Action action = () => fileSystem.Directory.EnumerateDirectories(XFS.Path(@"c:\d"));

            // Assert
            Assert.Throws<DirectoryNotFoundException>(action);
        }

        public static IEnumerable<object[]> GetPathsForMoving()
        {
            yield return new object[] { @"a:\folder1\", @"A:\folder3\", "file.txt", @"folder2\file2.txt" };
            yield return new object[] { @"A:\folder1\", @"A:\folder3\", "file.txt", @"folder2\file2.txt" };
            yield return new object[] { @"a:\folder1\", @"a:\folder3\", "file.txt", @"folder2\file2.txt" };
            yield return new object[] { @"A:\folder1\", @"a:\folder3\", "file.txt", @"folder2\file2.txt" };
            yield return new object[] { @"A:\folder1\", @"a:\folder3\", "file.txt", @"Folder2\file2.txt" };
            yield return new object[] { @"A:\folder1\", @"a:\folder3\", "file.txt", @"Folder2\fiLe2.txt" };
            yield return new object[] { @"A:\folder1\", @"a:\folder3\", "folder444\\file.txt", @"Folder2\fiLe2.txt" };
        }

        [Fact]
        public void Move_DirectoryExistsWithDifferentCase_DirectorySuccessfullyMoved()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(@"C:\OLD_LOCATION\Data");
            fileSystem.AddFile(@"C:\old_location\Data\someFile.txt", new MockFileData("abc"));

            // Act
            fileSystem.Directory.Move(@"C:\old_location", @"C:\NewLocation\");

            // Assert
            Assert.True(fileSystem.File.Exists(@"C:\NewLocation\Data\someFile.txt"));
        }

        [Theory]
        [MemberData(nameof(GetPathsForMoving))]
        public void MockDirectory_Move_ShouldMove(string sourceDirName, string destDirName, string filePathOne, string filePathTwo)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(sourceDirName + filePathOne) , new MockFileData("aaa") },
                { XFS.Path(sourceDirName + filePathTwo) , new MockFileData("bbb") },
            });

            // Act
            fileSystem.DirectoryInfo.FromDirectoryName(sourceDirName).MoveTo(destDirName);

            // Assert
            Assert.False(fileSystem.Directory.Exists(sourceDirName));
            Assert.True(fileSystem.File.Exists(XFS.Path(destDirName + filePathOne)));
            Assert.True(fileSystem.File.Exists(XFS.Path(destDirName + filePathTwo)));
        }

        [Fact]
        public void MockDirectory_GetCurrentDirectory_ShouldReturnValueFromFileSystemConstructor() {
            string directory = XFS.Path(@"D:\folder1\folder2");
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), directory);

            string actual = fileSystem.Directory.GetCurrentDirectory();

            Assert.Equal(directory, actual);
        }


        [Fact]
        public void MockDirectory_GetCurrentDirectory_ShouldReturnDefaultPathWhenNotSet() {
            string directory = Path.GetTempPath();
            MockFileSystem fileSystem = new MockFileSystem();

            string actual = fileSystem.Directory.GetCurrentDirectory();

            Assert.Equal(directory, actual);
        }

        [Fact]
        public void MockDirectory_SetCurrentDirectory_ShouldChangeCurrentDirectory() {
            string directory = XFS.Path(@"D:\folder1\folder2");
            MockFileSystem fileSystem = new MockFileSystem();

            // Precondition
            Assert.NotEqual(directory, fileSystem.Directory.GetCurrentDirectory());

            fileSystem.Directory.SetCurrentDirectory(directory);

            Assert.Equal(directory, fileSystem.Directory.GetCurrentDirectory());
        }

        [Fact]
        public void MockDirectory_GetParent_ShouldThrowArgumentNullExceptionIfPathIsNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action act = () => fileSystem.Directory.GetParent(null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void MockDirectory_GetParent_ShouldThrowArgumentExceptionIfPathIsEmpty()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action act = () => fileSystem.Directory.GetParent(string.Empty);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void MockDirectory_GetParent_ShouldReturnADirectoryInfoIfPathDoesNotExist()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            DirectoryInfoBase actualResult =  fileSystem.Directory.GetParent(XFS.Path(@"c:\directory\does\not\exist"));

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void MockDirectory_GetParent_ShouldThrowArgumentExceptionIfPathHasIllegalCharacters()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.True(true, "Path.GetInvalidChars() does not return anything on Mono");
            }

            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action act = () => fileSystem.Directory.GetParent(XFS.Path("c:\\director\ty\\has\\illegal\\character"));

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void MockDirectory_GetParent_ShouldReturnNullIfPathIsRoot()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\"));

            // Act
            DirectoryInfoBase actualResult = fileSystem.Directory.GetParent(XFS.Path(@"c:\"));

            // Assert
            Assert.Null(actualResult);
        }

        public static IEnumerable<string[]> MockDirectoryGetParentCases
        {
            get
            {
                yield return new [] { XFS.Path(@"c:\a"), XFS.Path(@"c:\") };
                yield return new [] { XFS.Path(@"c:\a\b\c\d"), XFS.Path(@"c:\a\b\c") };
                yield return new [] { XFS.Path(@"c:\a\b\c\d\"), XFS.Path(@"c:\a\b\c") };
            }
        }

        public void MockDirectory_GetParent_ShouldReturnTheParentWithoutTrailingDirectorySeparatorChar(string path, string expectedResult)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(path);

            // Act
            DirectoryInfoBase actualResult = fileSystem.Directory.GetParent(path);

            // Assert
            Assert.Equal(expectedResult, actualResult.FullName);
        }

        [Fact]
        public void MockDirectory_Move_ShouldThrowAnIOExceptionIfBothPathAreIdentical()
        {
            // Arrange
            string path = XFS.Path(@"c:\a");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(path);

            // Act
            Action action = () => fileSystem.Directory.Move(path, path);

            // Assert
            Assert.Throws<IOException>(action);
        }

        [Fact]
        public void MockDirectory_Move_ShouldThrowAnIOExceptionIfDirectoriesAreOnDifferentVolumes()
        {
            // Arrange
            string sourcePath = XFS.Path(@"c:\a");
            string destPath = XFS.Path(@"d:\v");
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(sourcePath);

            // Act
            Action action = () => fileSystem.Directory.Move(sourcePath, destPath);

            // Assert
            Assert.Throws<IOException>(action);
        }

        [Fact]
        public void MockDirectory_EnumerateFiles_ShouldReturnAllFilesBelowPathWhenPatternIsWildcardAndSearchOptionIsAllDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            IEnumerable<string> expected = new[]
            {
                XFS.Path(@"c:\a\a.txt"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\c.txt"),
                XFS.Path(@"c:\a\a\a.txt"),
                XFS.Path(@"c:\a\a\b.txt"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            IEnumerable<string> result = fileSystem.Directory.EnumerateFiles(XFS.Path(@"c:\a"), "*", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_EnumerateFiles_ShouldFilterByExtensionBasedSearchPattern()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[]
            {
                XFS.Path(@"c:\a.gif"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            IEnumerable<string> result = fileSystem.Directory.EnumerateFiles(XFS.Path(@"c:\"), "*.gif", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_EnumerateFileSystemEntries_ShouldReturnAllFilesBelowPathWhenPatternIsWildcardAndSearchOptionIsAllDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            IEnumerable<string> expected = new[]
            {
                XFS.Path(@"c:\a\a.txt"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\c.txt"),
                XFS.Path(@"c:\a\a\a.txt"),
                XFS.Path(@"c:\a\a\b.txt"),
                XFS.Path(@"c:\a\a\c.gif"),
                XFS.Path(@"c:\a\a\")
            };

            // Act
            IEnumerable<string> result = fileSystem.Directory.EnumerateFileSystemEntries(XFS.Path(@"c:\a"), "*", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectory_EnumerateFileSystemEntries_ShouldFilterByExtensionBasedSearchPattern()
        {
            // Arrange
            MockFileSystem fileSystem = SetupFileSystem();
            string[] expected = new[]
            {
                XFS.Path(@"c:\a.gif"),
                XFS.Path(@"c:\a\b.gif"),
                XFS.Path(@"c:\a\a\c.gif")
            };

            // Act
            IEnumerable<string> result = fileSystem.Directory.EnumerateFileSystemEntries(XFS.Path(@"c:\"), "*.gif", SearchOption.AllDirectories);

            // Assert
            Assert.Equal(result, expected);
        }
    }
}