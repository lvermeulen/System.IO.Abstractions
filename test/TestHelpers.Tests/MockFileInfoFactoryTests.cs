using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockFileInfoFactoryTests
    {
        [Fact]
        public void MockFileInfoFactory_FromFileName_ShouldReturnFileInfoForExistingFile()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData("Demo text content") },
                { @"c:\a\b\c.txt", new MockFileData("Demo text content") },
            });
            MockFileInfoFactory fileInfoFactory = new MockFileInfoFactory(fileSystem);

            // Act
            FileInfoBase result = fileInfoFactory.FromFileName(@"c:\a.txt");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MockFileInfoFactory_FromFileName_ShouldReturnFileInfoForNonExistantFile()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData("Demo text content") },
                { @"c:\a\b\c.txt", new MockFileData("Demo text content") },
            });
            MockFileInfoFactory fileInfoFactory = new MockFileInfoFactory(fileSystem);

            // Act
            FileInfoBase result = fileInfoFactory.FromFileName(@"c:\foo.txt");

            // Assert
            Assert.NotNull(result);
        }
    }
}