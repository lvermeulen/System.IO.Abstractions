using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockDriveInfoFactoryTests
    {
        [Fact]
        public void MockDriveInfoFactory_GetDrives_ShouldReturnDrives()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"Z:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"d:\Test"));
            MockDriveInfoFactory factory = new MockDriveInfoFactory(fileSystem);

            // Act
            DriveInfoBase[] actualResults = factory.GetDrives();

            IEnumerable<string> actualNames = actualResults.Select(d => d.Name);

            // Assert
            Assert.Equal(actualNames, new[] { @"C:\", @"Z:\", @"D:\" });
        }

        [Fact]
        public void MockDriveInfoFactory_GetDrives_ShouldReturnDrivesWithNoDuplicates()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"c:\Test2"));
            fileSystem.AddDirectory(XFS.Path(@"Z:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"d:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"d:\Test2"));
            MockDriveInfoFactory factory = new MockDriveInfoFactory(fileSystem);

            // Act
            DriveInfoBase[] actualResults = factory.GetDrives();

            IEnumerable<string> actualNames = actualResults.Select(d => d.Name);

            // Assert
            Assert.Equal(actualNames, new[] { @"C:\", @"Z:\", @"D:\" });
        }

        [Fact]
        public void MockDriveInfoFactory_GetDrives_ShouldReturnOnlyLocalDrives()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"C:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"Z:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"d:\Test"));
            fileSystem.AddDirectory(XFS.Path(@"\\anunc\share\Zzz"));
            MockDriveInfoFactory factory = new MockDriveInfoFactory(fileSystem);

            // Act
            DriveInfoBase[] actualResults = factory.GetDrives();

            IEnumerable<string> actualNames = actualResults.Select(d => d.Name);

            // Assert
            Assert.Equal(actualNames, new[] { @"C:\", @"Z:\", @"D:\" });
        }
    }
}
