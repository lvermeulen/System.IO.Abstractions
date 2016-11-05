using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Security.AccessControl;
using Xunit;

namespace System.IO.Abstractions.TestHelpers.Tests
{
    public class MockDirectoryArgumentPathTests
    {
        private static IEnumerable<Action<DirectoryBase>> GetFileSystemActionsForArgumentNullException()
        {
            yield return ds => ds.Delete(null);
            yield return ds => ds.Delete(null, true);
            yield return ds => ds.CreateDirectory(null);
            yield return ds => ds.CreateDirectory(null, new DirectorySecurity());
            yield return ds => ds.SetCreationTime(null, DateTime.Now);
            yield return ds => ds.SetCreationTimeUtc(null, DateTime.Now);
            yield return ds => ds.SetLastAccessTime(null, DateTime.Now);
            yield return ds => ds.SetLastAccessTimeUtc(null, DateTime.Now);
            yield return ds => ds.SetLastWriteTime(null, DateTime.Now);
            yield return ds => ds.SetLastWriteTimeUtc(null, DateTime.Now);
            yield return ds => ds.EnumerateDirectories(null);
            yield return ds => ds.EnumerateDirectories(null, "foo");
            yield return ds => ds.EnumerateDirectories(null, "foo", SearchOption.AllDirectories);
        }

        private static IEnumerable<object[]> GetFileSystemActionsForArgumentNullExceptionObjects()
        {
            foreach (var item in GetFileSystemActionsForArgumentNullException())
            {
                yield return new object[] { item };
            }
        }

        [Theory]
        [MemberData(nameof(GetFileSystemActionsForArgumentNullExceptionObjects))]
        public void Operations_ShouldThrowArgumentNullExceptionIfPathIsNull(Action<DirectoryBase> action)
        {
            // Arrange
            MockFileSystem fileSystem = new MockFileSystem();

            // Act
            Action wrapped = () => action(fileSystem.Directory);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(wrapped);
            Assert.Equal("path", exception.ParamName);
        }
    }
}
