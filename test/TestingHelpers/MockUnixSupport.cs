#if NETSTANDARD1_6 || NETCOREAPP1_0
using System.Runtime.InteropServices;
#endif
using System.Text.RegularExpressions;

namespace System.IO.Abstractions.TestingHelpers
{
    internal static class MockUnixSupport
    {
        internal static string Path(string path, Func<bool> isUnixF = null)
        {
            Func<bool> isUnix = isUnixF ?? IsUnixPlatform;

            if (isUnix())
            {
                path = Regex.Replace(path, @"^[a-zA-Z]:(?<path>.*)$", "${path}");
                path = path.Replace(@"\", "/");
            }

            return path;
        }

        internal static string Separator(Func<bool> isUnixF = null)
        {
            Func<bool> isUnix = isUnixF ?? IsUnixPlatform;
            return isUnix() ? "/" : @"\";
        }


#if NETSTANDARD1_6 || NETCOREAPP1_0

        internal static bool IsUnixPlatform() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
        internal static bool IsUnixPlatform()
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);

        }
#endif

    }
}