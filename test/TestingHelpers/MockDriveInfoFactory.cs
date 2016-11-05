using System.Collections.Generic;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockDriveInfoFactory : IDriveInfoFactory
    {
        private readonly IMockFileDataAccessor _mockFileSystem;

        public MockDriveInfoFactory(IMockFileDataAccessor mockFileSystem)
        {
            if (mockFileSystem == null)
            {
                throw new ArgumentNullException(nameof(mockFileSystem));
            }

            this._mockFileSystem = mockFileSystem;
        }

        public DriveInfoBase[] GetDrives()
        {
            HashSet<string> driveLetters = new HashSet<string>(new DriveEqualityComparer());
            foreach (string path in _mockFileSystem.AllPaths)
            {
                string pathRoot = _mockFileSystem.Path.GetPathRoot(path);
                driveLetters.Add(pathRoot);
            }

            List<DriveInfoBase> result = new List<DriveInfoBase>();
            foreach (string driveLetter in driveLetters)
            {
                try
                {
                    MockDriveInfo mockDriveInfo = new MockDriveInfo(_mockFileSystem, driveLetter);
                    result.Add(mockDriveInfo);
                }
                catch (ArgumentException)
                {
                    // invalid drives should be ignored
                }
            }

            return result.ToArray();
        }

        private string NormalizeDriveName(string driveName)
        {
            if (driveName.Length == 3 && driveName.EndsWith(@":\", StringComparison.OrdinalIgnoreCase))
            {
                return char.ToUpperInvariant(driveName[0]) + @":\";
            }

            if (driveName.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return driveName;
        }

        private class DriveEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x[1] == ':' && y[1] == ':')
                {
                    return char.ToUpperInvariant(x[0]) == char.ToUpperInvariant(y[0]);
                }

                return false;
            }

            public int GetHashCode(string obj) => obj.ToUpperInvariant().GetHashCode();
        }
    }
}
