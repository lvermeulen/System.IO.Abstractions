using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockFileReadAllLinesTests {
        [Fact]
        public void MockFile_ReadAllLines_ShouldReturnOriginalTextData()
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"), new MockFileData("Demo\r\ntext\ncontent\rvalue") },
                { XFS.Path(@"c:\something\other.gif"), new MockFileData(new byte[] { 0x21, 0x58, 0x3f, 0xa9 }) }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            string[] result = file.ReadAllLines(XFS.Path(@"c:\something\demo.txt"));

            // Assert
            Assert.Equal(
                new[] { "Demo", "text", "content", "value" },
                result);
        }

        [Fact]
        public void MockFile_ReadAllLines_ShouldReturnOriginalDataWithCustomEncoding()
        {
            // Arrange
            string text = "Hello\r\nthere\rBob\nBob!";
            byte[] encodedText = Encoding.BigEndianUnicode.GetBytes(text);
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\demo.txt"), new MockFileData(encodedText) }
            });

            MockFile file = new MockFile(fileSystem);

            // Act
            string[] result = file.ReadAllLines(XFS.Path(@"c:\something\demo.txt"), Encoding.BigEndianUnicode);

            // Assert
            Assert.Equal(
                new [] { "Hello", "there", "Bob", "Bob!" },
                result);
        }
    }
}