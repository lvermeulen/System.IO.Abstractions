namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockDirectoryData : MockFileData
    {
        public override bool IsDirectory => true;

        public MockDirectoryData() : base(string.Empty)
        {
            Attributes = FileAttributes.Directory;
        }
    }
}