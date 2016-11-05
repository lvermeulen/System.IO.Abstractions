using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace System.IO.Abstractions.TestingHelpers
{
    using XFS = MockUnixSupport;

    [Serializable]
    public class MockDirectory : DirectoryBase
    {
        private readonly FileBase _fileBase;

        private readonly IMockFileDataAccessor _mockFileDataAccessor;

        private string _currentDirectory;

        public MockDirectory(IMockFileDataAccessor mockFileDataAccessor, FileBase fileBase, string currentDirectory)
        {
            if (mockFileDataAccessor == null)
            {
                throw new ArgumentNullException(nameof(mockFileDataAccessor));
            }

            this._currentDirectory = currentDirectory;
            this._mockFileDataAccessor = mockFileDataAccessor;
            this._fileBase = fileBase;
        }

        public override DirectoryInfoBase CreateDirectory(string path) => CreateDirectory(path, null);

        public override DirectoryInfoBase CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.PATH_CANNOT_BE_THE_EMPTY_STRING_OR_ALL_WHITESPACE, nameof(path));
            }

            if (_mockFileDataAccessor.FileExists(path))
            {
                string message = string.Format(CultureInfo.InvariantCulture, @"Cannot create ""{0}"" because a file or directory with the same name already exists.", path);
                IOException ex = new IOException(message);
                ex.Data.Add("Path", path);
                throw ex;
            }

            path = EnsurePathEndsWithDirectorySeparator(_mockFileDataAccessor.Path.GetFullPath(path));

            if (!Exists(path))
            {
                _mockFileDataAccessor.AddDirectory(path);
            }

            MockDirectoryInfo created = new MockDirectoryInfo(_mockFileDataAccessor, path);
            return created;
        }

        public override void Delete(string path)
        {
            Delete(path, false);
        }

        public override void Delete(string path, bool recursive)
        {
            path = EnsurePathEndsWithDirectorySeparator(_mockFileDataAccessor.Path.GetFullPath(path));
            List<string> affectedPaths = _mockFileDataAccessor
                .AllPaths
                .Where(p => p.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!affectedPaths.Any())
            {
                throw new DirectoryNotFoundException(path + " does not exist or could not be found.");
            }

            if (!recursive &&
                affectedPaths.Count > 1)
            {
                throw new IOException("The directory specified by " + path + " is read-only, or recursive is false and " + path + " is not an empty directory.");
            }

            foreach (string affectedPath in affectedPaths)
            {
                _mockFileDataAccessor.RemoveFile(affectedPath);
            }
        }

        public override bool Exists(string path)
        {
            try
            {
                path = EnsurePathEndsWithDirectorySeparator(path);

                path = _mockFileDataAccessor.Path.GetFullPath(path);
                return _mockFileDataAccessor.AllDirectories.Any(p => p.Equals(path, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override DirectorySecurity GetAccessControl(string path)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override DateTime GetCreationTime(string path) => _fileBase.GetCreationTime(path);

        public override DateTime GetCreationTimeUtc(string path) => _fileBase.GetCreationTimeUtc(path);

        public override string GetCurrentDirectory() => _currentDirectory;

        public override string[] GetDirectories(string path) => GetDirectories(path, "*");

        public override string[] GetDirectories(string path, string searchPattern) => GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

        public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => EnumerateDirectories(path, searchPattern, searchOption).ToArray();

        public override string GetDirectoryRoot(string path) => Path.GetPathRoot(path);

        // Same as what the real framework does
        public override string[] GetFiles(string path) => GetFiles(path, "*");

        // Same as what the real framework does
        public override string[] GetFiles(string path, string searchPattern) => GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if(path == null)
            {
                throw new ArgumentNullException();
            }

            if (!Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.COULD_NOT_FIND_PART_OF_PATH_EXCEPTION, path));
            }

            return GetFilesInternal(_mockFileDataAccessor.AllFiles, path, searchPattern, searchOption);
        }

        private string[] GetFilesInternal(IEnumerable<string> files, string path, string searchPattern, SearchOption searchOption)
        {
            CheckSearchPattern(searchPattern);
            path = EnsurePathEndsWithDirectorySeparator(path);
            path = _mockFileDataAccessor.Path.GetFullPath(path);

            bool isUnix = XFS.IsUnixPlatform();

            string allDirectoriesPattern = isUnix
                ? @"([^<>:""/|?*]*/)*"
                : @"([^<>:""/\\|?*]*\\)*";

            string fileNamePattern;
            string pathPatternSpecial = null;
            if (searchPattern == "*")
            {
                fileNamePattern = isUnix ? @"[^/]*?/?" : @"[^\\]*?\\?";
            }
            else
            {
                fileNamePattern = Regex.Escape(searchPattern)
                    .Replace(@"\*", isUnix ? @"[^<>:""/|?*]*?" : @"[^<>:""/\\|?*]*?")
                    .Replace(@"\?", isUnix ? @"[^<>:""/|?*]?" : @"[^<>:""/\\|?*]?");

                string extension = Path.GetExtension(searchPattern);
                bool hasExtensionLengthOfThree = extension.Length == 4 && !extension.Contains("*") && !extension.Contains("?");
                if (hasExtensionLengthOfThree)
                {
                    string fileNamePatternSpecial = string.Format(CultureInfo.InvariantCulture, "{0}[^.]", fileNamePattern);
                    pathPatternSpecial = string.Format(
                        CultureInfo.InvariantCulture,
                        isUnix ? @"(?i:^{0}{1}{2}(?:/?)$)" : @"(?i:^{0}{1}{2}(?:\\?)$)",
                        Regex.Escape(path),
                        searchOption == SearchOption.AllDirectories ? allDirectoriesPattern : string.Empty,
                        fileNamePatternSpecial);
                }
            }

            string pathPattern = string.Format(
                CultureInfo.InvariantCulture,
                isUnix ? @"(?i:^{0}{1}{2}(?:/?)$)" : @"(?i:^{0}{1}{2}(?:\\?)$)",
                Regex.Escape(path),
                searchOption == SearchOption.AllDirectories ? allDirectoriesPattern : string.Empty,
                fileNamePattern);


            return files
                .Where(p =>
                    {
                        if (Regex.IsMatch(p, pathPattern))
                        {
                            return true;
                        }

                        if (pathPatternSpecial != null && Regex.IsMatch(p, pathPatternSpecial))
                        {
                            return true;
                        }

                        return false;
                    })
                .ToArray();
        }

        public override string[] GetFileSystemEntries(string path) => GetFileSystemEntries(path, "*");

        public override string[] GetFileSystemEntries(string path, string searchPattern)
        {
            string[] dirs = GetDirectories(path, searchPattern);
            string[] files = GetFiles(path, searchPattern);

            return dirs.Union(files).ToArray();
        }

        public override DateTime GetLastAccessTime(string path) => _fileBase.GetLastAccessTime(path);

        public override DateTime GetLastAccessTimeUtc(string path) => _fileBase.GetLastAccessTimeUtc(path);

        public override DateTime GetLastWriteTime(string path) => _fileBase.GetLastWriteTime(path);

        public override DateTime GetLastWriteTimeUtc(string path) => _fileBase.GetLastWriteTimeUtc(path);

        public override string[] GetLogicalDrives() => _mockFileDataAccessor
    .AllDirectories
    .Select(d => new MockDirectoryInfo(_mockFileDataAccessor, d).Root.FullName)
    .Select(r => r.ToLowerInvariant())
    .Distinct()
    .ToArray();

        public override DirectoryInfoBase GetParent(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.PATH_CANNOT_BE_THE_EMPTY_STRING_OR_ALL_WHITESPACE, nameof(path));
            }

            if (MockPath.HasIllegalCharacters(path, false))
            {
                throw new ArgumentException("Path contains invalid path characters.", nameof(path));
            }

            string absolutePath = _mockFileDataAccessor.Path.GetFullPath(path);
            string sepAsString = _mockFileDataAccessor.Path.DirectorySeparatorChar.ToString();

            int lastIndex = 0;
            if (absolutePath != sepAsString)
            {
                int startIndex = absolutePath.EndsWith(sepAsString, StringComparison.OrdinalIgnoreCase) ? absolutePath.Length - 1 : absolutePath.Length;
                lastIndex = absolutePath.LastIndexOf(_mockFileDataAccessor.Path.DirectorySeparatorChar, startIndex - 1);
                if (lastIndex < 0)
                {
                    return null;
                }
            }

            string parentPath = absolutePath.Substring(0, lastIndex);
            if (string.IsNullOrEmpty(parentPath))
            {
                return null;
            }

            MockDirectoryInfo parent = new MockDirectoryInfo(_mockFileDataAccessor, parentPath);
            return parent;
        }

        public override void Move(string sourceDirName, string destDirName)
        {
            string fullSourcePath = EnsurePathEndsWithDirectorySeparator(_mockFileDataAccessor.Path.GetFullPath(sourceDirName));
            string fullDestPath = EnsurePathEndsWithDirectorySeparator(_mockFileDataAccessor.Path.GetFullPath(destDirName));

            if (string.Equals(fullSourcePath, fullDestPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("Source and destination path must be different.");
            }

            string sourceRoot = _mockFileDataAccessor.Path.GetPathRoot(fullSourcePath);
            string destinationRoot = _mockFileDataAccessor.Path.GetPathRoot(fullDestPath);
            if (!string.Equals(sourceRoot, destinationRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("Source and destination path must have identical roots. Move will not work across volumes.");
            }

            //Make sure that the destination exists
            _mockFileDataAccessor.Directory.CreateDirectory(fullDestPath);

            //Recursively move all the subdirectories from the source into the destination directory
            string[] subdirectories = GetDirectories(fullSourcePath);
            foreach (string subdirectory in subdirectories)
            {
                string newSubdirPath = subdirectory.Replace(fullSourcePath, fullDestPath, StringComparison.OrdinalIgnoreCase);
                Move(subdirectory, newSubdirPath);
            }

            //Move the files in destination directory
            string[] files = GetFiles(fullSourcePath);
            foreach (string file in files)
            {
                string newFilePath = file.Replace(fullSourcePath, fullDestPath, StringComparison.OrdinalIgnoreCase);
                _mockFileDataAccessor.FileInfo.FromFileName(file).MoveTo(newFilePath);
            }

            //Delete the source directory
            Delete(fullSourcePath);
        }

        public override void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException(Properties.Resources.NOT_IMPLEMENTED_EXCEPTION);
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            _fileBase.SetCreationTime(path, creationTime);
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            _fileBase.SetCreationTimeUtc(path, creationTimeUtc);
        }

        public override void SetCurrentDirectory(string path)
        {
          _currentDirectory = path;
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            _fileBase.SetLastAccessTime(path, lastAccessTime);
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            _fileBase.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            _fileBase.SetLastWriteTime(path, lastWriteTime);
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            _fileBase.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public override IEnumerable<string> EnumerateDirectories(string path)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return EnumerateDirectories(path, "*");
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            return EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            _mockFileDataAccessor.PathVerifier.IsLegalAbsoluteOrRelative(path, nameof(path));

            path = EnsurePathEndsWithDirectorySeparator(path);

            if (!Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.COULD_NOT_FIND_PART_OF_PATH_EXCEPTION, path));
            }

            string[] dirs = GetFilesInternal(_mockFileDataAccessor.AllDirectories, path, searchPattern, searchOption);
            return dirs.Where(p => string.Compare(p, path, StringComparison.OrdinalIgnoreCase) != 0);
        }

        public override IEnumerable<string> EnumerateFiles(string path) => GetFiles(path);

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern) => GetFiles(path, searchPattern);

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) => GetFiles(path, searchPattern, searchOption);

        public override IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            List<string> fileSystemEntries = new List<string>(GetFiles(path));
            fileSystemEntries.AddRange(GetDirectories(path));
            return fileSystemEntries;
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            List<string> fileSystemEntries = new List<string>(GetFiles(path, searchPattern));
            fileSystemEntries.AddRange(GetDirectories(path, searchPattern));
            return fileSystemEntries;
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            List<string> fileSystemEntries = new List<string>(GetFiles(path, searchPattern, searchOption));
            fileSystemEntries.AddRange(GetDirectories(path, searchPattern, searchOption));
            return fileSystemEntries;
        }

        static string EnsurePathEndsWithDirectorySeparator(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        static void CheckSearchPattern(string searchPattern)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            const string TWO_DOTS = "..";
            Func<ArgumentException> createException = () => new ArgumentException(@"Search pattern cannot contain "".."" to move up directories and can be contained only internally in file/directory names, as in ""a..b"".", searchPattern);

            if (searchPattern.EndsWith(TWO_DOTS, StringComparison.OrdinalIgnoreCase))
            {
                throw createException();
            }

            int position;
            if ((position = searchPattern.IndexOf(TWO_DOTS, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                char characterAfterTwoDots = searchPattern[position + 2];
                if (characterAfterTwoDots == Path.DirectorySeparatorChar || characterAfterTwoDots == Path.AltDirectorySeparatorChar)
                {
                    throw createException();
                }
            }

            char[] invalidPathChars = Path.GetInvalidPathChars();
            if (searchPattern.IndexOfAny(invalidPathChars) > -1)
            {
                throw new ArgumentException(Properties.Resources.ILLEGAL_CHARACTERS_IN_PATH_EXCEPTION, nameof(searchPattern));
            }
        }
    }
}