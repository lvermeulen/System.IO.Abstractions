using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockDriveInfoTests
    {
        [Theory]
        [InlineData(@"c:")]
        [InlineData(@"c:\")]
        public void MockDriveInfo_Constructor_ShouldInitializeLocalWindowsDrives(string driveName)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));
            string path = XFS.Path(driveName);

            // Act
            MockDriveInfo driveInfo = new MockDriveInfo(fileSystem, path);

            // Assert
            Assert.Equal(@"C:\", driveInfo.Name);
        }

        [Fact]
        public void MockDriveInfo_Constructor_ShouldInitializeLocalWindowsDrives_SpecialForWindows()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.False(true, "Using XFS.Path transform c into c:.");
            }

            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));

            // Act
            MockDriveInfo driveInfo = new MockDriveInfo(fileSystem, "c");

            // Assert
            Assert.Equal(@"C:\", driveInfo.Name);
        }

        [Theory]
        [InlineData(@"\\unc\share")]
        [InlineData(@"\\unctoo")]
        public void MockDriveInfo_Constructor_ShouldThrowExceptionIfUncPath(string driveName)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action action = () => new MockDriveInfo(fileSystem, XFS.Path(driveName));

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void MockDriveInfo_RootDirectory_ShouldReturnTheDirectoryBase()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));
            MockDriveInfo driveInfo = new MockDriveInfo(fileSystem, "c:");
            string expectedDirectory = XFS.Path(@"C:\");

            // Act
            DirectoryInfoBase actualDirectory = driveInfo.RootDirectory;

            // Assert
            Assert.Equal(expectedDirectory, actualDirectory.FullName);
        }
    }
}
