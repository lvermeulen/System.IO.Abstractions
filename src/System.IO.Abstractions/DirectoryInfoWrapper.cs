using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace System.IO.Abstractions
{
    [Serializable]
    public class DirectoryInfoWrapper : DirectoryInfoBase
    {
        private readonly DirectoryInfo _instance;

        public DirectoryInfoWrapper(DirectoryInfo instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this._instance = instance;
        }

        public override void Delete()
        {
            _instance.Delete();
        }

        public override void Refresh()
        {
            _instance.Refresh();
        }

        public override FileAttributes Attributes
        {
            get { return _instance.Attributes; }
            set { _instance.Attributes = value; }
        }

        public override DateTime CreationTime
        {
            get { return _instance.CreationTime; }
            set { _instance.CreationTime = value; }
        }

        public override DateTime CreationTimeUtc
        {
            get { return _instance.CreationTimeUtc; }
            set { _instance.CreationTimeUtc = value; }
        }

        public override bool Exists => _instance.Exists;

        public override string Extension => _instance.Extension;

        public override string FullName => _instance.FullName;

        public override DateTime LastAccessTime
        {
            get { return _instance.LastAccessTime; }
            set { _instance.LastAccessTime = value; }
        }

        public override DateTime LastAccessTimeUtc
        {
            get { return _instance.LastAccessTimeUtc; }
            set { _instance.LastAccessTimeUtc = value; }
        }

        public override DateTime LastWriteTime
        {
            get { return _instance.LastWriteTime; }
            set { _instance.LastWriteTime = value; }
        }

        public override DateTime LastWriteTimeUtc
        {
            get { return _instance.LastWriteTimeUtc; }
            set { _instance.LastWriteTimeUtc = value; }
        }

        public override string Name => _instance.Name;

        public override void Create()
        {
            _instance.Create();
        }

        public override void Create(DirectorySecurity directorySecurity)
        {
            _instance.Create();
        }

        public override DirectoryInfoBase CreateSubdirectory(string path) => new DirectoryInfoWrapper(_instance.CreateSubdirectory(path));

        public override DirectoryInfoBase CreateSubdirectory(string path, DirectorySecurity directorySecurity) => new DirectoryInfoWrapper(_instance.CreateSubdirectory(path));

        public override void Delete(bool recursive)
        {
            _instance.Delete(recursive);
        }

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories() => _instance.EnumerateDirectories().Select(directoryInfo => new DirectoryInfoWrapper(directoryInfo));

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories(string searchPattern) => _instance.EnumerateDirectories(searchPattern).Select(directoryInfo => new DirectoryInfoWrapper(directoryInfo));

        public override IEnumerable<DirectoryInfoBase> EnumerateDirectories(string searchPattern, SearchOption searchOption) => _instance.EnumerateDirectories(searchPattern, searchOption).Select(directoryInfo => new DirectoryInfoWrapper(directoryInfo));

        public override IEnumerable<FileInfoBase> EnumerateFiles() => _instance.EnumerateFiles().Select(fileInfo => new FileInfoWrapper(fileInfo));

        public override IEnumerable<FileInfoBase> EnumerateFiles(string searchPattern) => _instance.EnumerateFiles(searchPattern).Select(fileInfo => new FileInfoWrapper(fileInfo));

        public override IEnumerable<FileInfoBase> EnumerateFiles(string searchPattern, SearchOption searchOption) => _instance.EnumerateFiles(searchPattern, searchOption).Select(fileInfo => new FileInfoWrapper(fileInfo));

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos() => _instance.EnumerateFileSystemInfos().WrapFileSystemInfos();

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern) => _instance.EnumerateFileSystemInfos(searchPattern).WrapFileSystemInfos();

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption) => _instance.EnumerateFileSystemInfos(searchPattern, searchOption).WrapFileSystemInfos();

        public override DirectorySecurity GetAccessControl() => _instance.GetAccessControl();

        public override DirectorySecurity GetAccessControl(AccessControlSections includeSections) => _instance.GetAccessControl(includeSections);

        public override DirectoryInfoBase[] GetDirectories() => _instance.GetDirectories().WrapDirectories();

        public override DirectoryInfoBase[] GetDirectories(string searchPattern) => _instance.GetDirectories(searchPattern).WrapDirectories();

        public override DirectoryInfoBase[] GetDirectories(string searchPattern, SearchOption searchOption) => _instance.GetDirectories(searchPattern, searchOption).WrapDirectories();

        public override FileInfoBase[] GetFiles() => _instance.GetFiles().WrapFiles();

        public override FileInfoBase[] GetFiles(string searchPattern) => _instance.GetFiles(searchPattern).WrapFiles();

        public override FileInfoBase[] GetFiles(string searchPattern, SearchOption searchOption) => _instance.GetFiles(searchPattern, searchOption).WrapFiles();

        public override FileSystemInfoBase[] GetFileSystemInfos() => _instance.GetFileSystemInfos().WrapFileSystemInfos();

        public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern) => _instance.GetFileSystemInfos(searchPattern).WrapFileSystemInfos();

        public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern, SearchOption searchOption) => _instance.GetFileSystemInfos(searchPattern, searchOption).WrapFileSystemInfos();

        public override void MoveTo(string destDirName)
        {
            _instance.MoveTo(destDirName);
        }

        public override void SetAccessControl(DirectorySecurity directorySecurity)
        {
            _instance.SetAccessControl(directorySecurity);
        }

        public override DirectoryInfoBase Parent => _instance.Parent;

        public override DirectoryInfoBase Root => _instance.Root;

        public override string ToString() => _instance.ToString();
    }
}