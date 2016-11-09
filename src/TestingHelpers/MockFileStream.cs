namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileStream : MemoryStream
    {
        private readonly IMockFileDataAccessor _mockFileDataAccessor;
        private readonly string _path;

        public MockFileStream(IMockFileDataAccessor mockFileDataAccessor, string path, bool forAppend = false)
        {
            if (mockFileDataAccessor == null)
            {
                throw new ArgumentNullException(nameof(mockFileDataAccessor));
            }

            this._mockFileDataAccessor = mockFileDataAccessor;
            this._path = path;

            if (mockFileDataAccessor.FileExists(path))
            {
                /* only way to make an expandable MemoryStream that starts with a particular content */
                byte[] data = mockFileDataAccessor.GetFile(path).Contents;
                if (data != null && data.Length > 0)
                {
                    Write(data, 0, data.Length);
                    Seek(0, forAppend
                        ? SeekOrigin.End
                        : SeekOrigin.Begin);
                }
            }
            else
            {
                mockFileDataAccessor.AddFile(path, new MockFileData(new byte[] { }));
            }
        }

#if NETSTANDARD1_6 || NETCOREAPP1_0
        public void Close()
#else
        public override void Close()
#endif
        {
            InternalFlush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            InternalFlush();
        }

        private void InternalFlush()
        {
            if (_mockFileDataAccessor.FileExists(_path))
            {
                MockFileData mockFileData = _mockFileDataAccessor.GetFile(_path);
                /* reset back to the beginning .. */
                Seek(0, SeekOrigin.Begin);
                /* .. read everything out */
                byte[] data = new byte[Length];
                Read(data, 0, (int)Length);
                /* .. put it in the mock system */
                mockFileData.Contents = data;
            }
        }
    }
}