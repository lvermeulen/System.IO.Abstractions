namespace System.IO.Abstractions
{
    [Serializable]
    public class PathWrapper : PathBase
    {
        public override char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

        public override char DirectorySeparatorChar => Path.DirectorySeparatorChar;

        [Obsolete("Please use GetInvalidPathChars or GetInvalidFileNameChars instead.")]
        public override char[] InvalidPathChars
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char PathSeparator => Path.PathSeparator;

        public override char VolumeSeparatorChar => Path.VolumeSeparatorChar;

        public override string ChangeExtension(string path, string extension) => Path.ChangeExtension(path, extension);

        public override string Combine(params string[] paths) => Path.Combine(paths);

        public override string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public override string Combine(string path1, string path2, string path3) => Path.Combine(path1, path2, path3);

        public override string Combine(string path1, string path2, string path3, string path4) => Path.Combine(path1, path2, path3, path4);

        public override string GetDirectoryName(string path) => Path.GetDirectoryName(path);

        public override string GetExtension(string path) => Path.GetExtension(path);

        public override string GetFileName(string path) => Path.GetFileName(path);

        public override string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

        public override string GetFullPath(string path) => Path.GetFullPath(path);

        public override char[] GetInvalidFileNameChars() => Path.GetInvalidFileNameChars();

        public override char[] GetInvalidPathChars() => Path.GetInvalidPathChars();

        public override string GetPathRoot(string path) => Path.GetPathRoot(path);

        public override string GetRandomFileName() => Path.GetRandomFileName();

        public override string GetTempFileName() => Path.GetTempFileName();

        public override string GetTempPath() => Path.GetTempPath();

        public override bool HasExtension(string path) => Path.HasExtension(path);

        public override bool IsPathRooted(string path) => Path.IsPathRooted(path);
    }
}
