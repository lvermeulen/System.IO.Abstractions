using System.Xml.Serialization;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class FileSystemTests
    {
        [Fact]
        public void Is_Serializable()
        {
            FileSystem fileSystem = new FileSystem();
            MemoryStream memoryStream = new MemoryStream();

            var serializer = new XmlSerializer(fileSystem.GetType());
            serializer.Serialize(memoryStream, fileSystem);

            Assert.True(memoryStream.Length > 0, "Length didn't increase after serialization task.");
        }
    }
}
