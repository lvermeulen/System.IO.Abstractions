using System.Security.AccessControl;

namespace System.IO.Abstractions
{
    [Serializable]
    public class FileInfoWrapper : FileInfoBase
    {
        private readonly FileInfo _instance;

        public FileInfoWrapper(FileInfo instance)
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

        public override StreamWriter AppendText() => _instance.AppendText();

        public override FileInfoBase CopyTo(string destFileName) => _instance.CopyTo(destFileName);

        public override FileInfoBase CopyTo(string destFileName, bool overwrite) => _instance.CopyTo(destFileName, overwrite);

        public override Stream Create() => _instance.Create();

        public override StreamWriter CreateText() => _instance.CreateText();

        public override void Decrypt()
        {
        }

        public override void Encrypt()
        {
        }

        public override FileSecurity GetAccessControl() => _instance.GetAccessControl();

        public override FileSecurity GetAccessControl(AccessControlSections includeSections) => _instance.GetAccessControl(includeSections);

        public override void MoveTo(string destFileName)
        {
            _instance.MoveTo(destFileName);
        }

        public override Stream Open(FileMode mode) => _instance.Open(mode);

        public override Stream Open(FileMode mode, FileAccess access) => _instance.Open(mode, access);

        public override Stream Open(FileMode mode, FileAccess access, FileShare share) => _instance.Open(mode, access, share);

        public override Stream OpenRead() => _instance.OpenRead();

        public override StreamReader OpenText() => _instance.OpenText();

        public override Stream OpenWrite() => _instance.OpenWrite();

        public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName)
        {
            throw new NotImplementedException();
        }

        public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            throw new NotImplementedException();
        }

        public override void SetAccessControl(FileSecurity fileSecurity)
        {
            _instance.SetAccessControl(fileSecurity);
        }

        public override DirectoryInfoBase Directory => _instance.Directory;

        public override string DirectoryName => _instance.DirectoryName;

        public override bool IsReadOnly
        {
            get { return _instance.IsReadOnly; }
            set { _instance.IsReadOnly = value; }
        }

        public override long Length => _instance.Length;
    }
}