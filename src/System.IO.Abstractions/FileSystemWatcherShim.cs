#if NETSTANDARD1_6 || NETCOREAPP1_0
using System.ComponentModel;
#endif

namespace System.IO.Abstractions
{
    public class FileSystemWatcherShim : FileSystemWatcher
    {
        public FileSystemWatcherShim()
        { }

        public FileSystemWatcherShim(string path) 
            : base(path)
        { }

        public FileSystemWatcherShim(string path, string filter) 
            : base(path, filter)
        { }

#if NETSTANDARD1_6 || NETCOREAPP1_0
        public ISite Site { get; set; }
#else
#endif
    }
}
