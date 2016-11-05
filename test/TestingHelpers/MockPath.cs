using System.Collections.Generic;
using System.Linq;

namespace System.IO.Abstractions.TestingHelpers
{
    /// <summary>
    /// PathWrapper calls direct to Path but all this does is string manipulation so we can inherit directly from PathWrapper as no IO is done
    /// </summary>
    [Serializable]
    public class MockPath : PathWrapper
    {
        private readonly IMockFileDataAccessor _mockFileDataAccessor;

        private static readonly char[] s_invalidAdditionalPathChars = { '*', '?' };

        public MockPath(IMockFileDataAccessor mockFileDataAccessor)
        {
            if (mockFileDataAccessor == null)
            {
                throw new ArgumentNullException(nameof(mockFileDataAccessor));
            }

            this._mockFileDataAccessor = mockFileDataAccessor;
        }

        public override string GetFullPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), Properties.Resources.VALUE_CANNOT_BE_NULL);
            }

            if (path.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.THE_PATH_IS_NOT_OF_A_LEGAL_FORM, nameof(path));
            }

            path = path.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);

            bool isUnc =
                path.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith(@"//", StringComparison.OrdinalIgnoreCase);

            string root = GetPathRoot(path);

            bool hasTrailingSlash = path.Length > 1 && path[path.Length - 1] == DirectorySeparatorChar;

            string[] pathSegments;

            if (root.Length == 0)
            {
                // relative path on the current drive or volume
                path = _mockFileDataAccessor.Directory.GetCurrentDirectory() + DirectorySeparatorChar + path;
                pathSegments = GetSegments(path);
            }
            else if (isUnc)
            {
                // unc path
                pathSegments = GetSegments(path);
                if (pathSegments.Length < 2)
                {
                    throw new ArgumentException(@"The UNC path should be of the form \\server\share.", nameof(path));
                }
            }
            else if (@"\".Equals(root, StringComparison.OrdinalIgnoreCase) || @"/".Equals(root, StringComparison.OrdinalIgnoreCase))
            {
                // absolute path on the current drive or volume
                pathSegments = GetSegments(GetPathRoot(_mockFileDataAccessor.Directory.GetCurrentDirectory()), path);
            }
            else
            {
                pathSegments = GetSegments(path);
            }

            // unc paths need at least two segments, the others need one segment
            bool isUnixRooted =
                _mockFileDataAccessor.Directory.GetCurrentDirectory()
                    .StartsWith(DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase);

            int minPathSegments = isUnc
                ? 2
                : isUnixRooted ? 0 : 1;

            Stack<string> stack = new Stack<string>();
            foreach (string segment in pathSegments)
            {
                if ("..".Equals(segment, StringComparison.OrdinalIgnoreCase))
                {
                    // only pop, if afterwards are at least the minimal amount of path segments
                    if (stack.Count > minPathSegments)
                    {
                        stack.Pop();
                    }
                }
                else if (".".Equals(segment, StringComparison.OrdinalIgnoreCase))
                {
                    // ignore .
                }
                else
                {
                    stack.Push(segment);
                }
            }

            string fullPath = string.Join(DirectorySeparatorChar.ToString(), stack.Reverse().ToArray());

            if (hasTrailingSlash)
            {
                fullPath += DirectorySeparatorChar;
            }

            if (isUnixRooted && !isUnc)
            {
                fullPath = DirectorySeparatorChar + fullPath;
            }
            else if (isUnixRooted)
            {
                fullPath = @"//" + fullPath;
            }
            else if (isUnc)
            {
                fullPath = @"\\" + fullPath;
            }

            return fullPath;
        }

        private string[] GetSegments(params string[] paths) => paths.SelectMany(path => path.Split(new[] { DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)).ToArray();

        public override string GetTempFileName()
        {
            string fileName = _mockFileDataAccessor.Path.GetRandomFileName();
            string tempDir = _mockFileDataAccessor.Path.GetTempPath();

            string fullPath = _mockFileDataAccessor.Path.Combine(tempDir, fileName);

            _mockFileDataAccessor.AddFile(fullPath, new MockFileData(string.Empty));

            return fullPath;
        }

        internal static bool HasIllegalCharacters(string path, bool checkAdditional)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (checkAdditional)
            {
                return path.IndexOfAny(Path.GetInvalidPathChars().Concat(s_invalidAdditionalPathChars).ToArray()) >= 0;
            }

            return path.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
        }

        internal static void CheckInvalidPathChars(string path, bool checkAdditional = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (HasIllegalCharacters(path, checkAdditional))
            {
                throw new ArgumentException(Properties.Resources.ILLEGAL_CHARACTERS_IN_PATH_EXCEPTION);
            }
        }
    }
}
