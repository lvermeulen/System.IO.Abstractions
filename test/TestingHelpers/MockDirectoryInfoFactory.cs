namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockDirectoryInfoFactory : IDirectoryInfoFactory
    {
        readonly IMockFileDataAccessor _mockFileSystem;

        public MockDirectoryInfoFactory(IMockFileDataAccessor mockFileSystem)
        {
            this._mockFileSystem = mockFileSystem;
        }

        public DirectoryInfoBase FromDirectoryName(string directoryName) => new MockDirectoryInfo(_mockFileSystem, directoryName);
    }
}