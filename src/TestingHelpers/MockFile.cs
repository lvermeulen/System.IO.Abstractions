﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFile : FileBase
    {
        private readonly IMockFileDataAccessor _mockFileDataAccessor;
        private readonly MockPath _mockPath;

        public MockFile(IMockFileDataAccessor mockFileDataAccessor)
        {
            if (mockFileDataAccessor == null)
            {
                throw new ArgumentNullException(nameof(mockFileDataAccessor));
            }

            this._mockFileDataAccessor = mockFileDataAccessor;
            _mockPath = new MockPath(mockFileDataAccessor);
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(contents, "contents");

            AppendAllLines(path, contents, MockFileData.DefaultEncoding);
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            string concatContents = contents.Aggregate("", (a, b) => a + b + Environment.NewLine);
            AppendAllText(path, concatContents, encoding);
        }

        public override void AppendAllText(string path, string contents)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            AppendAllText(path, contents, MockFileData.DefaultEncoding);
        }

        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (!_mockFileDataAccessor.FileExists(path))
            {
                string dir = _mockFileDataAccessor.Path.GetDirectoryName(path);
                if (!_mockFileDataAccessor.Directory.Exists(dir))
                {
                    throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.COULD_NOT_FIND_PART_OF_PATH_EXCEPTION, path));
                }

                _mockFileDataAccessor.AddFile(path, new MockFileData(contents, encoding));
            }
            else
            {
                MockFileData file = _mockFileDataAccessor.GetFile(path);
                byte[] bytesToAppend = encoding.GetBytes(contents);
                file.Contents = file.Contents.Concat(bytesToAppend).ToArray();
            }
        }

        public override StreamWriter AppendText(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (_mockFileDataAccessor.FileExists(path))
            {
                StreamWriter sw = new StreamWriter(OpenWrite(path));
                sw.BaseStream.Seek(0, SeekOrigin.End); //push the stream pointer at the end for append.
                return sw;
            }

            return new StreamWriter(Create(path));
        }

        public override void Copy(string sourceFileName, string destFileName)
        {
            Copy(sourceFileName, destFileName, false);
        }

        public override void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName == null)
            {
                throw new ArgumentNullException(nameof(sourceFileName), Properties.Resources.FILENAME_CANNOT_BE_NULL);
            }

            if (destFileName == null)
            {
                throw new ArgumentNullException(nameof(destFileName), Properties.Resources.FILENAME_CANNOT_BE_NULL);
            }

            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(sourceFileName, nameof(sourceFileName));
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(destFileName, nameof(destFileName));

            string directoryNameOfDestination = _mockPath.GetDirectoryName(destFileName);
            if (!_mockFileDataAccessor.Directory.Exists(directoryNameOfDestination))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.COULD_NOT_FIND_PART_OF_PATH_EXCEPTION, destFileName));
            }

            bool fileExists = _mockFileDataAccessor.FileExists(destFileName);
            if (fileExists)
            {
                if (!overwrite)
                {
                    throw new IOException($"The file {destFileName} already exists.");
                }

                _mockFileDataAccessor.RemoveFile(destFileName);
            }

            MockFileData sourceFile = _mockFileDataAccessor.GetFile(sourceFileName);
            _mockFileDataAccessor.AddFile(destFileName, sourceFile);
        }

        public override Stream Create(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null.");
            }
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.AddFile(path, new MockFileData(new byte[0]));
            Stream stream = OpenWrite(path);
            return stream;
        }

        public override Stream Create(string path, int bufferSize)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override Stream Create(string path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override StreamWriter CreateText(string path) => new StreamWriter(Create(path));

        public override void Decrypt(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            new MockFileInfo(_mockFileDataAccessor, path).Decrypt();
        }

        public override void Delete(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.RemoveFile(path);
        }

        public override void Encrypt(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            new MockFileInfo(_mockFileDataAccessor, path).Encrypt();
        }

        public override bool Exists(string path) => _mockFileDataAccessor.FileExists(path) && !_mockFileDataAccessor.AllDirectories.Any(d => d.Equals(path, StringComparison.OrdinalIgnoreCase));

        public override FileSecurity GetAccessControl(string path)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        /// <summary>
        /// Gets the <see cref="FileAttributes"/> of the file on the path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
        /// <exception cref="ArgumentException"><paramref name="path"/> is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> represents a file and is invalid, such as being on an unmapped drive, or the file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> represents a directory and is invalid, such as being on an unmapped drive, or the directory cannot be found.</exception>
        /// <exception cref="IOException">This file is being used by another process.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public override FileAttributes GetAttributes(string path)
        {
            if (path?.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.THE_PATH_IS_NOT_OF_A_LEGAL_FORM, nameof(path));
            }

            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            MockFileData possibleFileData = _mockFileDataAccessor.GetFile(path);
            FileAttributes result;
            if (possibleFileData != null)
            {
                result = possibleFileData.Attributes;
            }
            else
            {
                DirectoryInfoBase directoryInfo = _mockFileDataAccessor.DirectoryInfo.FromDirectoryName(path);
                if (directoryInfo.Exists)
                {
                    result = directoryInfo.Attributes;
                }
                else
                {
                    DirectoryInfoBase parentDirectoryInfo = directoryInfo.Parent;
                    if (!parentDirectoryInfo.Exists)
                    {
                        throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture,
                            Properties.Resources.COULD_NOT_FIND_PART_OF_PATH_EXCEPTION, path));
                    }

                    throw new FileNotFoundException($"Could not find file '{path}'.");
                }
            }

            return result;
        }

        public override DateTime GetCreationTime(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.CreationTime.LocalDateTime, () => MockFileData.DefaultDateTimeOffset.LocalDateTime);
        }

        public override DateTime GetCreationTimeUtc(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.CreationTime.UtcDateTime, () => MockFileData.DefaultDateTimeOffset.UtcDateTime);
        }

        public override DateTime GetLastAccessTime(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.LastAccessTime.LocalDateTime, () => MockFileData.DefaultDateTimeOffset.LocalDateTime);
        }

        public override DateTime GetLastAccessTimeUtc(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.LastAccessTime.UtcDateTime, () => MockFileData.DefaultDateTimeOffset.UtcDateTime);
        }

        public override DateTime GetLastWriteTime(string path) {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.LastWriteTime.LocalDateTime, () => MockFileData.DefaultDateTimeOffset.LocalDateTime);
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return GetTimeFromFile(path, data => data.LastWriteTime.UtcDateTime, () => MockFileData.DefaultDateTimeOffset.UtcDateTime);
        }

        private DateTime GetTimeFromFile(string path, Func<MockFileData, DateTime> existingFileFunction, Func<DateTime> nonExistingFileFunction)
        {
            MockFileData file = _mockFileDataAccessor.GetFile(path);
            DateTime result = file != null 
                ? existingFileFunction(file) 
                : nonExistingFileFunction();

            return result;
        }

        public override void Move(string sourceFileName, string destFileName)
        {
            if (sourceFileName == null)
            {
                throw new ArgumentNullException(nameof(sourceFileName), Properties.Resources.FILENAME_CANNOT_BE_NULL);
            }

            if (destFileName == null)
            {
                throw new ArgumentNullException(nameof(destFileName), Properties.Resources.FILENAME_CANNOT_BE_NULL);
            }

            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(sourceFileName, nameof(sourceFileName));
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(destFileName, nameof(destFileName));

            if (_mockFileDataAccessor.GetFile(destFileName) != null)
            {
                throw new IOException("A file can not be created if it already exists.");
            }

            MockFileData sourceFile = _mockFileDataAccessor.GetFile(sourceFileName);

            if (sourceFile == null)
            {
                throw new FileNotFoundException($"The file \"{sourceFileName}\" could not be found.", sourceFileName);
            }

            DirectoryInfoBase destDir = _mockFileDataAccessor.Directory.GetParent(destFileName);
            if (!destDir.Exists)
            {
                throw new DirectoryNotFoundException("Could not find a part of the path.");
            }

            _mockFileDataAccessor.AddFile(destFileName, new MockFileData(sourceFile.Contents));
            _mockFileDataAccessor.RemoveFile(sourceFileName);
        }

        public override Stream Open(string path, FileMode mode)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return Open(path, mode, (mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite), FileShare.None);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return Open(path, mode, access, FileShare.None);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            bool exists = _mockFileDataAccessor.FileExists(path);

            if (mode == FileMode.CreateNew && exists)
            {
                throw new IOException($"The file '{path}' already exists.");
            }

            if ((mode == FileMode.Open || mode == FileMode.Truncate) && !exists)
            {
                throw new FileNotFoundException(path);
            }

            if (!exists || mode == FileMode.CreateNew)
            {
                return Create(path);
            }

            if (mode == FileMode.Create || mode == FileMode.Truncate)
            {
                Delete(path);
                return Create(path);
            }

            int length = _mockFileDataAccessor.GetFile(path).Contents.Length;
            Stream stream = OpenWrite(path);

            if (mode == FileMode.Append)
            {
                stream.Seek(length, SeekOrigin.Begin);
            }

            return stream;
        }

        public override Stream OpenRead(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public override StreamReader OpenText(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return new StreamReader(
                OpenRead(path));
        }

        public override Stream OpenWrite(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return new MockFileStream(_mockFileDataAccessor, path);
        }

        public override byte[] ReadAllBytes(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return _mockFileDataAccessor.GetFile(path).Contents;
        }

        public override string[] ReadAllLines(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (!_mockFileDataAccessor.FileExists(path))
            {
                throw new FileNotFoundException($"Can't find {path}");
            }

            return _mockFileDataAccessor
                .GetFile(path)
                .TextContents
                .SplitLines();
        }

        public override string[] ReadAllLines(string path, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (!_mockFileDataAccessor.FileExists(path))
            {
                throw new FileNotFoundException($"Can't find {path}");
            }

            return encoding
                .GetString(_mockFileDataAccessor.GetFile(path).Contents)
                .SplitLines();
        }

        public override string ReadAllText(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (!_mockFileDataAccessor.FileExists(path))
            {
                throw new FileNotFoundException($"Can't find {path}");
            }

            return ReadAllText(path, MockFileData.DefaultEncoding);
        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            return ReadAllTextInternal(path, encoding);
        }

        public override IEnumerable<string> ReadLines(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return ReadAllLines(path);
        }

        public override IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(encoding, "encoding");

            return ReadAllLines(path, encoding);
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void SetAttributes(string path, FileAttributes fileAttributes)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).Attributes = fileAttributes;
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).CreationTime = new DateTimeOffset(creationTime);
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).CreationTime = new DateTimeOffset(creationTimeUtc, TimeSpan.Zero);
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).LastAccessTime = new DateTimeOffset(lastAccessTime);
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).LastAccessTime = new DateTimeOffset(lastAccessTimeUtc, TimeSpan.Zero);
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).LastWriteTime = new DateTimeOffset(lastWriteTime);
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            _mockFileDataAccessor.GetFile(path).LastWriteTime = new DateTimeOffset(lastWriteTimeUtc, TimeSpan.Zero);
        }

        /// <summary>
        /// Creates a new file, writes the specified byte array to the file, and then closes the file.
        /// If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="bytes">The bytes to write to the file. </param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/> or contents is empty.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// path specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <remarks>
        /// Given a byte array and a file path, this method opens the specified file, writes the contents of the byte array to the file, and then closes the file.
        /// </remarks>
        public override void WriteAllBytes(string path, byte[] bytes)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null.");
            }

            VerifyValueIsNotNull(bytes, "bytes");

            _mockFileDataAccessor.AddFile(path, new MockFileData(bytes));
        }

        /// <summary>
        /// Creates a new file, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException">Either <paramref name="path"/> or <paramref name="contents"/> is <see langword="null"/>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="path"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <remarks>
        /// <para>
        ///     If the target file already exists, it is overwritten.
        /// </para>
        /// <para>
        ///     You can use this method to create the contents for a collection class that takes an <see cref="IEnumerable{T}"/> in its constructor, such as a <see cref="List{T}"/>, <see cref="HashSet{T}"/>, or a <see cref="SortedSet{T}"/> class.
        /// </para>
        /// </remarks>
        public override void WriteAllLines(string path, IEnumerable<string> contents)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(contents, "contents");

            WriteAllLines(path, contents, MockFileData.DefaultEncoding);
        }

        /// <summary>
        /// Creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException">Either <paramref name="path"/>, <paramref name="contents"/>, or <paramref name="encoding"/> is <see langword="null"/>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="path"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <remarks>
        /// <para>
        ///     If the target file already exists, it is overwritten.
        /// </para>
        /// <para>
        ///     You can use this method to create a file that contains the following:
        /// <list type="bullet">
        /// <item>
        /// <description>The results of a LINQ to Objects query on the lines of a file, as obtained by using the ReadLines method.</description>
        /// </item>
        /// <item>
        /// <description>The contents of a collection that implements an <see cref="IEnumerable{T}"/> of strings.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(contents, "contents");
            VerifyValueIsNotNull(encoding, "encoding");

            StringBuilder sb = new StringBuilder();
            foreach (string line in contents)
            {
                sb.AppendLine(line);
            }

            WriteAllText(path, sb.ToString(), encoding);
        }

        /// <summary>
        /// Creates a new file, writes the specified string array to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string array to write to the file.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException">Either <paramref name="path"/> or <paramref name="contents"/> is <see langword="null"/>.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="path"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <remarks>
        /// <para>
        ///     If the target file already exists, it is overwritten.
        /// </para>
        /// <para>
        ///     The default behavior of the WriteAllLines method is to write out data using UTF-8 encoding without a byte order mark (BOM). If it is necessary to include a UTF-8 identifier, such as a byte order mark, at the beginning of a file, use the <see cref="FileBase.WriteAllLines(string,string[],System.Text.Encoding)"/> method overload with <see cref="UTF8Encoding"/> encoding.
        /// </para>
        /// <para>
        ///     Given a string array and a file path, this method opens the specified file, writes the string array to the file using the specified encoding,
        ///     and then closes the file.
        /// </para>
        /// </remarks>
        public override void WriteAllLines(string path, string[] contents)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(contents, "contents");

            WriteAllLines(path, contents, MockFileData.DefaultEncoding);
        }

        /// <summary>
        /// Creates a new file, writes the specified string array to the file by using the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string array to write to the file.</param>
        /// <param name="encoding">An <see cref="Encoding"/> object that represents the character encoding applied to the string array.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException">Either <paramref name="path"/> or <paramref name="contents"/> is <see langword="null"/>.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="path"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <remarks>
        /// <para>
        ///     If the target file already exists, it is overwritten.
        /// </para>
        /// <para>
        ///     Given a string array and a file path, this method opens the specified file, writes the string array to the file using the specified encoding,
        ///     and then closes the file.
        /// </para>
        /// </remarks>
        public override void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(contents, "contents");
            VerifyValueIsNotNull(encoding, "encoding");

            WriteAllLines(path, new List<string>(contents), encoding);
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string to write to the file. </param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/> or contents is empty.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// path specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <remarks>
        /// This method uses UTF-8 encoding without a Byte-Order Mark (BOM), so using the <see cref="M:Encoding.GetPreamble"/> method will return an empty byte array.
        /// If it is necessary to include a UTF-8 identifier, such as a byte order mark, at the beginning of a file, use the <see cref="FileBase.WriteAllText(string,string,System.Text.Encoding)"/> method overload with <see cref="UTF8Encoding"/> encoding.
        /// <para>
        /// Given a string and a file path, this method opens the specified file, writes the string to the file, and then closes the file.
        /// </para>
        /// </remarks>
        public override void WriteAllText(string path, string contents)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            WriteAllText(path, contents, MockFileData.DefaultEncoding);
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string to write to the file. </param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/> or contents is empty.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// path specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <remarks>
        /// Given a string and a file path, this method opens the specified file, writes the string to the file using the specified encoding, and then closes the file.
        /// The file handle is guaranteed to be closed by this method, even if exceptions are raised.
        /// </remarks>
        public override void WriteAllText(string path, string contents, Encoding encoding)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));
            VerifyValueIsNotNull(path, "path");

            if (_mockFileDataAccessor.Directory.Exists(path))
            {
                throw new UnauthorizedAccessException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.ACCESS_TO_THE_PATH_IS_DENIED, path));
            }

            MockFileData data = contents == null ? new MockFileData(new byte[0]) : new MockFileData(contents, encoding);
            _mockFileDataAccessor.AddFile(path, data);
        }

        internal static string ReadAllBytes(byte[] contents, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream(contents))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private string ReadAllTextInternal(string path, Encoding encoding)
        {
            MockFileData mockFileData = _mockFileDataAccessor.GetFile(path);
            return ReadAllBytes(mockFileData.Contents, encoding);
        }

        private void VerifyValueIsNotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, Properties.Resources.VALUE_CANNOT_BE_NULL);
            }
        }
    }
}