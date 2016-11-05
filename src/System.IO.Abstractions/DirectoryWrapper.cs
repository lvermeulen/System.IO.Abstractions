using System.Collections.Generic;
using System.Security.AccessControl;

namespace System.IO.Abstractions
{
    [Serializable]
    public class DirectoryWrapper : DirectoryBase
    {
        public override DirectoryInfoBase CreateDirectory(string path) => Directory.CreateDirectory(path);

        public override DirectoryInfoBase CreateDirectory(string path, DirectorySecurity directorySecurity) => Directory.CreateDirectory(path);

        public override void Delete(string path)
        {
            Directory.Delete(path);
        }

        public override void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public override bool Exists(string path) => Directory.Exists(path);

        public override DirectorySecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTime(string path) => Directory.GetCreationTime(path);

        public override DateTime GetCreationTimeUtc(string path) => Directory.GetCreationTimeUtc(path);

        public override string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public override string[] GetDirectories(string path) => Directory.GetDirectories(path);

        public override string[] GetDirectories(string path, string searchPattern) => Directory.GetDirectories(path, searchPattern);

        public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => Directory.GetDirectories(path, searchPattern, searchOption);

        public override string GetDirectoryRoot(string path) => Directory.GetDirectoryRoot(path);

        public override string[] GetFiles(string path) => Directory.GetFiles(path);

        public override string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

        public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => Directory.GetFiles(path, searchPattern, searchOption);

        public override string[] GetFileSystemEntries(string path) => Directory.GetFileSystemEntries(path);

        public override string[] GetFileSystemEntries(string path, string searchPattern) => Directory.GetFileSystemEntries(path, searchPattern);

        public override DateTime GetLastAccessTime(string path) => Directory.GetLastAccessTime(path);

        public override DateTime GetLastAccessTimeUtc(string path) => Directory.GetLastAccessTimeUtc(path);

        public override DateTime GetLastWriteTime(string path) => Directory.GetLastWriteTime(path);

        public override DateTime GetLastWriteTimeUtc(string path) => Directory.GetLastWriteTimeUtc(path);

        public override string[] GetLogicalDrives()
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase GetParent(string path) => Directory.GetParent(path);

        public override void Move(string sourceDirName, string destDirName)
        {
            Directory.Move(sourceDirName, destDirName);
        }

        public override void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            Directory.SetCreationTime(path, creationTime);
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            Directory.SetCreationTimeUtc(path, creationTimeUtc);
        }

        public override void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            Directory.SetLastAccessTime(path, lastAccessTime);
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            Directory.SetLastAccessTime(path, lastWriteTime);
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public override IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateDirectories(path, searchPattern, searchOption);

        public override IEnumerable<string> EnumerateFiles(string path)
        {
           return Directory.EnumerateFiles(path);
        }

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFiles(path, searchPattern, searchOption);

        public override IEnumerable<string> EnumerateFileSystemEntries(string path) => Directory.EnumerateFileSystemEntries(path);

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
    }
}