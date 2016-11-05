namespace System.IO.Abstractions
{
    [Serializable]
    public class FileSystem : IFileSystem
    {
        DirectoryBase _directory;
        public DirectoryBase Directory => _directory ?? (_directory = new DirectoryWrapper());

        FileBase _file;
        public FileBase File => _file ?? (_file = new FileWrapper());

        FileInfoFactory _fileInfoFactory;
        public IFileInfoFactory FileInfo => _fileInfoFactory ?? (_fileInfoFactory = new FileInfoFactory());

        PathBase _path;
        public PathBase Path => _path ?? (_path = new PathWrapper());

        DirectoryInfoFactory _directoryInfoFactory;
        public IDirectoryInfoFactory DirectoryInfo => _directoryInfoFactory ?? (_directoryInfoFactory = new DirectoryInfoFactory());

        private readonly Lazy<DriveInfoFactory> _driveInfoFactory = new Lazy<DriveInfoFactory>(() => new DriveInfoFactory());

        public IDriveInfoFactory DriveInfo => _driveInfoFactory.Value;
    }
}