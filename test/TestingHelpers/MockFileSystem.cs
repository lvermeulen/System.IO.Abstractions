using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.IO.Abstractions.TestingHelpers
{
    using XFS = MockUnixSupport;

    [Serializable]
    public class MockFileSystem : IFileSystem, IMockFileDataAccessor
    {
        private readonly IDictionary<string, MockFileData> _files;
        private readonly FileBase _file;
        private readonly DirectoryBase _directory;
        private readonly IFileInfoFactory _fileInfoFactory;
        private readonly PathBase _pathField;
        private readonly IDirectoryInfoFactory _directoryInfoFactory;
        private readonly IDriveInfoFactory _driveInfoFactory;

        [NonSerialized]
        private readonly PathVerifier _pathVerifier;

        public MockFileSystem() : this(null) { }

        public MockFileSystem(IDictionary<string, MockFileData> files, string currentDirectory = "")
        {
            if (string.IsNullOrEmpty(currentDirectory))
            {
                currentDirectory = IO.Path.GetTempPath();
            }

            _pathVerifier = new PathVerifier(this);

            this._files = new Dictionary<string, MockFileData>(StringComparer.OrdinalIgnoreCase);
            _pathField = new MockPath(this);
            _file = new MockFile(this);
            _directory = new MockDirectory(this, _file, currentDirectory);
            _fileInfoFactory = new MockFileInfoFactory(this);
            _directoryInfoFactory = new MockDirectoryInfoFactory(this);
            _driveInfoFactory = new MockDriveInfoFactory(this);

            if (files != null)
            {
                foreach (KeyValuePair<string, MockFileData> entry in files)
                {
                    AddFile(entry.Key, entry.Value);
                }
            }
        }

        public FileBase File => _file;

        public DirectoryBase Directory => _directory;

        public IFileInfoFactory FileInfo => _fileInfoFactory;

        public PathBase Path => _pathField;

        public IDirectoryInfoFactory DirectoryInfo => _directoryInfoFactory;

        public IDriveInfoFactory DriveInfo => _driveInfoFactory;

        public PathVerifier PathVerifier => _pathVerifier;

        private string FixPath(string path)
        {
            string pathSeparatorFixed = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return _pathField.GetFullPath(pathSeparatorFixed);
        }

        public MockFileData GetFile(string path)
        {
            path = FixPath(path);

            return GetFileWithoutFixingPath(path);
        }

        public void AddFile(string path, MockFileData mockFile)
        {
            string fixedPath = FixPath(path);
            lock (_files)
            {
                if (FileExists(fixedPath))
                {
                    bool isReadOnly = (_files[fixedPath].Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                    bool isHidden = (_files[fixedPath].Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                    if (isReadOnly || isHidden)
                    {
                        throw new UnauthorizedAccessException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.ACCESS_TO_THE_PATH_IS_DENIED, path));
                    }
                }

                string directoryPath = Path.GetDirectoryName(fixedPath);

                if (!_directory.Exists(directoryPath))
                {
                    AddDirectory(directoryPath);
                }

                _files[fixedPath] = mockFile;
            }
        }

        public void AddDirectory(string path)
        {
            string fixedPath = FixPath(path);
            string separator = XFS.Separator();

            lock (_files)
            {
                if (FileExists(path) &&
                    (_files[fixedPath].Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new UnauthorizedAccessException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.ACCESS_TO_THE_PATH_IS_DENIED, path));
                }

                int lastIndex = 0;

                bool isUnc =
                    path.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(@"//", StringComparison.OrdinalIgnoreCase);

                if (isUnc)
                {
                    //First, confirm they aren't trying to create '\\server\'
                    lastIndex = path.IndexOf(separator, 2, StringComparison.OrdinalIgnoreCase);
                    if (lastIndex < 0)
                    {
                        throw new ArgumentException(@"The UNC path should be of the form \\server\share.", nameof(path));
                    }

                    /*
                     * Although CreateDirectory(@"\\server\share\") is not going to work in real code, we allow it here for the purposes of setting up test doubles.
                     * See PR https://github.com/tathamoddie/System.IO.Abstractions/pull/90 for conversation
                     */
                }

                while ((lastIndex = path.IndexOf(separator, lastIndex + 1, StringComparison.OrdinalIgnoreCase)) > -1)
                {
                    string segment = path.Substring(0, lastIndex + 1);
                    if (!_directory.Exists(segment))
                    {
                        _files[segment] = new MockDirectoryData();
                    }
                }

                string s = path.EndsWith(separator, StringComparison.OrdinalIgnoreCase) ? path : path + separator;
                _files[s] = new MockDirectoryData();
            }
        }

        public void RemoveFile(string path)
        {
            path = FixPath(path);

            lock (_files)
            {
                _files.Remove(path);
            }
        }

        public bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            path = FixPath(path);

            lock (_files)
            {
                return _files.ContainsKey(path);
            }
        }

        public IEnumerable<string> AllPaths
        {
            get
            {
                lock (_files)
                {
                    return _files.Keys.ToArray();
                }
            }
        }

        public IEnumerable<string> AllFiles
        {
            get
            {
                lock (_file)
                {
                    return _files.Where(f => !f.Value.IsDirectory).Select(f => f.Key).ToArray();
                }
            }
        }

        public IEnumerable<string> AllDirectories
        {
            get
            {
                lock (_files)
                {
                    return _files.Where(f => f.Value.IsDirectory).Select(f => f.Key).ToArray();
                }
            }
        }

        private MockFileData GetFileWithoutFixingPath(string path)
        {
            lock (_files)
            {
                MockFileData result;
                _files.TryGetValue(path, out result);
                return result;
            }
        }
    }
}
