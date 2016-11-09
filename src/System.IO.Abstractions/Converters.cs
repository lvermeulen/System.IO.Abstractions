using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.IO.Abstractions
{
    internal static class Converters
    {
        internal static FileSystemInfoBase[] WrapFileSystemInfos(this IEnumerable<FileSystemInfo> input) => input
    .Select<FileSystemInfo, FileSystemInfoBase>(item =>
    {
        if (item is FileInfo)
        {
            return (FileInfoBase)item;
        }

        if (item is DirectoryInfo)
        {
            return (DirectoryInfoBase)item;
        }

        throw new NotImplementedException($"The type {item.GetType().AssemblyQualifiedName} is not recognized by the System.IO.Abstractions library.");
    })
    .ToArray();

        internal static DirectoryInfoBase[] WrapDirectories(this IEnumerable<DirectoryInfo> input) => input.Select(f => (DirectoryInfoBase)f).ToArray();

        internal static FileInfoBase[] WrapFiles(this IEnumerable<FileInfo> input) => input.Select(f => (FileInfoBase)f).ToArray();
    }
}