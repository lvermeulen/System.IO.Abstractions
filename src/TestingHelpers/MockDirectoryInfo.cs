using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockDirectoryInfo : DirectoryInfoBase
    {
        private readonly IMockFileDataAccessor _mockFileDataAccessor;
        private readonly string _directoryPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDirectoryInfo"/> class.
        /// </summary>
        /// <param name="mockFileDataAccessor">The mock file data accessor.</param>
        /// <param name="directoryPath">The directory path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mockFileDataAccessor"/> or <paramref name="directoryPath"/> is <see langref="null"/>.</exception>
        public MockDirectoryInfo(IMockFileDataAccessor mockFileDataAccessor, string directoryPath)
        {
            if (mockFileDataAccessor == null)
            {
                throw new ArgumentNullException(nameof(mockFileDataAccessor));
            }

            this._mockFileDataAccessor = mockFileDataAccessor;

            directoryPath = mockFileDataAccessor.Path.GetFullPath(directoryPath);

            this._directoryPath = EnsurePathEndsWithDirectorySeparator(directoryPath);
        }

        MockFileData MockFileData => _mockFileDataAccessor.GetFile(_directoryPath);

        public override void Delete()
        {
            _mockFileDataAccessor.Directory.Delete(_directoryPath);
        }

        public override void Refresh()
        {
        }

        public override FileAttributes Attributes
        {
            get { return MockFileData.Attributes; }
            set { MockFileData.Attributes = value; }
        }

        public override DateTime CreationTime
        {
            get { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override DateTime CreationTimeUtc
        {
            get { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override bool Exists => _mockFileDataAccessor.Directory.Exists(FullName);

        public override string Extension => Path.GetExtension(_directoryPath);

        public override string FullName
        {
            get
            {
                string root = _mockFileDataAccessor.Path.GetPathRoot(_directoryPath);
                if (string.Equals(_directoryPath, root, StringComparison.OrdinalIgnoreCase))
                {
                    // drives have the trailing slash
                    return _directoryPath;
                }

                // directories do not have a trailing slash
                return _directoryPath.TrimEnd('\\').TrimEnd('/');
            }
        }

        public override DateTime LastAccessTime
        {
            get { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override DateTime LastAccessTimeUtc
        {
            get {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _directoryPath);
                }
                return MockFileData.LastAccessTime.UtcDateTime;
            }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override DateTime LastWriteTime
        {
            get { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override DateTime LastWriteTimeUtc
        {
            get { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
            set { throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION); }
        }

        public override string Name => new MockPath(_mockFileDataAccessor).GetFileName(_directoryPath.TrimEnd('\\'));

        public override void Create()
        {
            _mockFileDataAccessor.Directory.CreateDirectory(FullName);
        }

        public override void Create(DirectorySecurity directorySecurity)
        {
            _mockFileDataAccessor.Directory.CreateDirectory(FullName, directorySecurity);
        }

        public override DirectoryInfoBase CreateSubdirectory(string path) => _mockFileDataAccessor.Directory.CreateDirectory(Path.Combine(FullName, path));

        public override DirectoryInfoBase CreateSubdirectory(string path, DirectorySecurity directorySecurity) => _mockFileDataAccessor.Directory.CreateDirectory(Path.Combine(FullName, path), directorySecurity);

        public override void Delete(bool recursive)
        {
            _mockFileDataAccessor.Directory.Delete(_directoryPath, recursive);
        }

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories() => GetDirectories();

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories(string searchPattern) => GetDirectories(searchPattern);

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories(string searchPattern, SearchOption searchOption) => GetDirectories(searchPattern, searchOption);

        public override IEnumerable<FileInfoBase> EnumerateFiles() => GetFiles();

        public override IEnumerable<FileInfoBase> EnumerateFiles(string searchPattern) => GetFiles(searchPattern);

        public override IEnumerable<FileInfoBase> EnumerateFiles(string searchPattern, SearchOption searchOption) => GetFiles(searchPattern, searchOption);

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos() => GetFileSystemInfos();

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern) => GetFileSystemInfos(searchPattern);

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption) => GetFileSystemInfos(searchPattern, searchOption);

        public override DirectorySecurity GetAccessControl() => _mockFileDataAccessor.Directory.GetAccessControl(_directoryPath);

        public override DirectorySecurity GetAccessControl(AccessControlSections includeSections) => _mockFileDataAccessor.Directory.GetAccessControl(_directoryPath, includeSections);

        public override DirectoryInfoBase[] GetDirectories() => ConvertStringsToDirectories(_mockFileDataAccessor.Directory.GetDirectories(_directoryPath));

        public override DirectoryInfoBase[] GetDirectories(string searchPattern) => ConvertStringsToDirectories(_mockFileDataAccessor.Directory.GetDirectories(_directoryPath, searchPattern));

        public override DirectoryInfoBase[] GetDirectories(string searchPattern, SearchOption searchOption) => ConvertStringsToDirectories(_mockFileDataAccessor.Directory.GetDirectories(_directoryPath, searchPattern, searchOption));

        private DirectoryInfoBase[] ConvertStringsToDirectories(IEnumerable<string> paths) => paths
    .Select(path => new MockDirectoryInfo(_mockFileDataAccessor, path))
    .Cast<DirectoryInfoBase>()
    .ToArray();

        public override FileInfoBase[] GetFiles() => ConvertStringsToFiles(_mockFileDataAccessor.Directory.GetFiles(FullName));

        public override FileInfoBase[] GetFiles(string searchPattern) => ConvertStringsToFiles(_mockFileDataAccessor.Directory.GetFiles(FullName, searchPattern));

        public override FileInfoBase[] GetFiles(string searchPattern, SearchOption searchOption) => ConvertStringsToFiles(_mockFileDataAccessor.Directory.GetFiles(FullName, searchPattern, searchOption));

        FileInfoBase[] ConvertStringsToFiles(IEnumerable<string> paths) => paths
      .Select(_mockFileDataAccessor.FileInfo.FromFileName)
      .ToArray();

        public override FileSystemInfoBase[] GetFileSystemInfos() => GetFileSystemInfos("*");

        public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern) => GetFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);

        public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern, SearchOption searchOption) => GetDirectories(searchPattern, searchOption).OfType<FileSystemInfoBase>().Concat(GetFiles(searchPattern, searchOption)).ToArray();

        public override void MoveTo(string destDirName)
        {
            _mockFileDataAccessor.Directory.Move(_directoryPath, destDirName);
        }

        public override void SetAccessControl(DirectorySecurity directorySecurity)
        {
            _mockFileDataAccessor.Directory.SetAccessControl(_directoryPath, directorySecurity);
        }

        public override DirectoryInfoBase Parent => _mockFileDataAccessor.Directory.GetParent(_directoryPath);

        public override DirectoryInfoBase Root => new MockDirectoryInfo(_mockFileDataAccessor, _mockFileDataAccessor.Directory.GetDirectoryRoot(FullName));

        private string EnsurePathEndsWithDirectorySeparator(string path)
        {
            if (!path.EndsWith(_mockFileDataAccessor.Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                path += _mockFileDataAccessor.Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}
