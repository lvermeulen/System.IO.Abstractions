namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileInfoFactory : IFileInfoFactory
    {
        private readonly IMockFileDataAccessor _mockFileSystem;

        public MockFileInfoFactory(IMockFileDataAccessor mockFileSystem)
        {
            if (mockFileSystem == null)
            {
                throw new ArgumentNullException(nameof(mockFileSystem));
            }

            this._mockFileSystem = mockFileSystem;
        }

        public FileInfoBase FromFileName(string fileName) => new MockFileInfo(_mockFileSystem, fileName);
    }
}