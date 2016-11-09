using System.Security.AccessControl;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileInfo : FileInfoBase
    {
        private readonly IMockFileDataAccessor _mockFileSystem;
        private string _path;

        public MockFileInfo(IMockFileDataAccessor mockFileSystem, string path)
        {
            if (mockFileSystem == null)
            {
                throw new ArgumentNullException(nameof(mockFileSystem));
            }

            this._mockFileSystem = mockFileSystem;
            this._path = path;
        }

        MockFileData MockFileData => _mockFileSystem.GetFile(_path);

        public override void Delete()
        {
            _mockFileSystem.RemoveFile(_path);
        }

        public override void Refresh()
        {
        }

        public override FileAttributes Attributes
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.Attributes;
            }
            set { MockFileData.Attributes = value; }
        }

        public override DateTime CreationTime
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.CreationTime.DateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.CreationTime = value;
            }
        }

        public override DateTime CreationTimeUtc
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.CreationTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.CreationTime = value.ToLocalTime();
            }
        }

        public override bool Exists => MockFileData != null;

        public override string Extension => Path.GetExtension(_path);

        public override string FullName => _path;

        public override DateTime LastAccessTime
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.LastAccessTime.DateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.LastAccessTime = value;
            }
        }

        public override DateTime LastAccessTimeUtc
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.LastAccessTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.LastAccessTime = value;
            }
        }

        public override DateTime LastWriteTime
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.LastWriteTime.DateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.LastWriteTime = value;
            }
        }

        public override DateTime LastWriteTimeUtc
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.LastWriteTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                MockFileData.LastWriteTime = value.ToLocalTime();
            }
        }

        public override string Name => new MockPath(_mockFileSystem).GetFileName(_path);

        public override StreamWriter AppendText()
        {
            if (MockFileData == null)
            {
                throw new FileNotFoundException("File not found", _path);
            }
            return new StreamWriter(new MockFileStream(_mockFileSystem, FullName, true));
            //return ((MockFileDataModifier) MockFileData).AppendText();
        }

        public override FileInfoBase CopyTo(string destFileName)
        {
            new MockFile(_mockFileSystem).Copy(FullName, destFileName);
            return _mockFileSystem.FileInfo.FromFileName(destFileName);
        }

        public override FileInfoBase CopyTo(string destFileName, bool overwrite)
        {
            new MockFile(_mockFileSystem).Copy(FullName, destFileName, overwrite);
            return _mockFileSystem.FileInfo.FromFileName(destFileName);
        }

        public override Stream Create() => new MockFile(_mockFileSystem).Create(FullName);

        public override StreamWriter CreateText() => new MockFile(_mockFileSystem).CreateText(FullName);

        public override void Decrypt()
        {
            if (MockFileData == null)
            {
                throw new FileNotFoundException("File not found", _path);
            }
            byte[] contents = MockFileData.Contents;
            for (int i = 0; i < contents.Length; i++)
            {
                contents[i] ^= (byte)(i % 256);
            }
        }

        public override void Encrypt()
        {
            if (MockFileData == null)
            {
                throw new FileNotFoundException("File not found", _path);
            }
            byte[] contents = MockFileData.Contents;
            for(int i = 0; i < contents.Length; i++)
            {
                contents[i] ^= (byte) (i % 256);
            }
        }

        public override FileSecurity GetAccessControl()
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override FileSecurity GetAccessControl(AccessControlSections includeSections)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void MoveTo(string destFileName)
        {
            FileInfoBase movedFileInfo = CopyTo(destFileName);
            Delete();
            _path = movedFileInfo.FullName;
        }

        public override Stream Open(FileMode mode) => new MockFile(_mockFileSystem).Open(FullName, mode);

        public override Stream Open(FileMode mode, FileAccess access) => new MockFile(_mockFileSystem).Open(FullName, mode, access);

        public override Stream Open(FileMode mode, FileAccess access, FileShare share) => new MockFile(_mockFileSystem).Open(FullName, mode, access, share);

        public override Stream OpenRead() => new MockFileStream(_mockFileSystem, _path);

        public override StreamReader OpenText() => new StreamReader(OpenRead());

        public override Stream OpenWrite() => new MockFileStream(_mockFileSystem, _path);

        public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void SetAccessControl(FileSecurity fileSecurity)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override DirectoryInfoBase Directory => _mockFileSystem.DirectoryInfo.FromDirectoryName(DirectoryName);

        public override string DirectoryName => Path.GetDirectoryName(_path);

        public override bool IsReadOnly
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return (MockFileData.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                if(value)
                {
                    MockFileData.Attributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    MockFileData.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
        }

        public override long Length
        {
            get
            {
                if (MockFileData == null)
                {
                    throw new FileNotFoundException("File not found", _path);
                }
                return MockFileData.Contents.Length;
            }
        }
    }
}