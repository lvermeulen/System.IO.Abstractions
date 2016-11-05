namespace System.IO.Abstractions
{
    [Serializable]
    internal class FileInfoFactory : IFileInfoFactory
    {
        public FileInfoBase FromFileName(string fileName)
        {
            FileInfo realFileInfo = new FileInfo(fileName);
            return new FileInfoWrapper(realFileInfo);
        }
    }
}