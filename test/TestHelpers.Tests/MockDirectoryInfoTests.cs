using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockDirectoryInfoTests
    {
        public static IEnumerable<object[]> MockDirectoryInfoGetExtensionCases
        {
            get
            {
                yield return new object[] { XFS.Path(@"c:\temp") };
                yield return new object[] { XFS.Path(@"c:\temp\") };
            }
        }

        [Theory]
        [MemberData(nameof(MockDirectoryInfoGetExtensionCases))]
        public void MockDirectoryInfo_GetExtension_ShouldReturnEmptyString(string directoryPath)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, directoryPath);

            // Act
            string result = directoryInfo.Extension;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        public static IEnumerable<object[]> MockDirectoryInfoExistsCases
        {
            get
            {
                yield return new object[]{ XFS.Path(@"c:\temp\folder"), true };
                yield return new object[]{ XFS.Path(@"c:\temp\folder\notExistant"), false };
            }
        }

        [Theory]
        [MemberData(nameof(MockDirectoryInfoExistsCases))]
        public void MockDirectoryInfo_Exists(string path, bool expected)
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World")}
            });
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, path);

            bool result = directoryInfo.Exists;

            Assert.Equal(result, expected);
        }

        [Fact]
        public void MockDirectoryInfo_FullName_ShouldReturnFullNameWithoutIncludingTrailingPathDelimiter()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    XFS.Path(@"c:\temp\folder\file.txt"),
                        new MockFileData("Hello World")
                }
            });
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

            string result = directoryInfo.FullName;

            Assert.Equal(result, XFS.Path(@"c:\temp\folder"));
        }

        [Fact]
        public void MockDirectoryInfo_GetFileSystemInfos_ShouldReturnBothDirectoriesAndFiles()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() }
            });

            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            FileSystemInfoBase[] result = directoryInfo.GetFileSystemInfos();

            Assert.Equal(result.Length, 2);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFileSystemInfos_ShouldReturnBothDirectoriesAndFiles()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() }
            });

            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            FileSystemInfoBase[] result = directoryInfo.EnumerateFileSystemInfos().ToArray();

            Assert.Equal(result.Length, 2);
        }

        [Fact]
        public void MockDirectoryInfo_GetFileSystemInfos_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() },
                { XFS.Path(@"c:\temp\folder\older"), new MockDirectoryData() }
            });

            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            FileSystemInfoBase[] result = directoryInfo.GetFileSystemInfos("f*");

            Assert.Equal(result.Length, 2);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFileSystemInfos_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() },
                { XFS.Path(@"c:\temp\folder\older"), new MockDirectoryData() }
            });

            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            FileSystemInfoBase[] result = directoryInfo.EnumerateFileSystemInfos("f*", SearchOption.AllDirectories).ToArray();

            Assert.Equal(result.Length, 2);
        }

        [Fact]
        public void MockDirectoryInfo_GetParent_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\a\b\c"));
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\a\b\c"));

            // Act
            DirectoryInfoBase result = directoryInfo.Parent;

            // Assert
            Assert.Equal(XFS.Path(@"c:\a\b"), result.FullName);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFiles_ShouldReturnAllFiles()
        {
          // Arrange
          MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                //Files "above" in folder we're querying
                { XFS.Path(@"c:\temp\a.txt"), "" },

                //Files in the folder we're querying
                { XFS.Path(@"c:\temp\folder\b.txt"), "" },
                { XFS.Path(@"c:\temp\folder\c.txt"), "" },

                //Files "below" the folder we're querying
                { XFS.Path(@"c:\temp\folder\deeper\d.txt"), "" }
            });

          // Act
          MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

          // Assert
          Assert.Equal(new[]{"b.txt", "c.txt"}, directoryInfo.EnumerateFiles().ToList().Select(x => x.Name).ToArray());
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateDirectories_ShouldReturnAllDirectories()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                //A file we want to ignore entirely
                { XFS.Path(@"c:\temp\folder\a.txt"), "" },

                //Some files in sub folders (which we also want to ignore entirely)
                { XFS.Path(@"c:\temp\folder\b\file.txt"), "" },
                { XFS.Path(@"c:\temp\folder\c\other.txt"), "" },
            });
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

            // Act
            string[] directories = directoryInfo.EnumerateDirectories().Select(a => a.Name).ToArray();

            // Assert
            Assert.Equal(new[] { "b", "c" }, directories);
        }

        public static IEnumerable<object[]> MockDirectoryInfoFullNameData
        {
            get
            {
                yield return new object[] { XFS.Path(@"c:\temp\\folder"), XFS.Path(@"c:\temp\folder") };
                yield return new object[] { XFS.Path(@"c:\temp//folder"), XFS.Path(@"c:\temp\folder") };
                yield return new object[] { XFS.Path(@"c:\temp//\\///folder"), XFS.Path(@"c:\temp\folder") };
                yield return new object[] { XFS.Path(@"\\unc\folder"), XFS.Path(@"\\unc\folder") };
                yield return new object[] { XFS.Path(@"\\unc/folder\\foo"), XFS.Path(@"\\unc\folder\foo") };
            }
        }

        [Theory]
        [MemberData(nameof(MockDirectoryInfoFullNameData))]
        public void MockDirectoryInfo_FullName_ShouldReturnNormalizedPath(string directoryPath, string expectedFullName)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\a.txt"), "" }
            });
            MockDirectoryInfo directoryInfo = new MockDirectoryInfo(fileSystem, directoryPath);

            // Act
            string actualFullName = directoryInfo.FullName;

            // Assert
            Assert.Equal(expectedFullName, actualFullName);
        }

        [Fact]
        public void MockDirectoryInfo_Constructor_ShouldThrowArgumentNullException_IfArgumentDirectoryIsNull()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
           Action action = () => new MockDirectoryInfo(fileSystem, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Value cannot be null.", exception.Message);
        }

        [Fact]
        public void MockDirectoryInfo_Constructor_ShouldThrowArgumentNullException_IfArgumentFileSystemIsNull()
        {
            // Arrange
            // nothing to do

            // Act
            Action action = () => new MockDirectoryInfo(null, XFS.Path(@"c:\foo\bar\folder"));

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void MockDirectoryInfo_Constructor_ShouldThrowArgumentException_IfArgumentDirectoryIsEmpty()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => new MockDirectoryInfo(fileSystem, string.Empty);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("The path is not of a legal form.", exception.Message);
        }
    }
}