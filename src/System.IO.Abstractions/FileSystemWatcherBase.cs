using System.ComponentModel;

namespace System.IO.Abstractions
{
    [Serializable]
    public abstract class FileSystemWatcherBase : IDisposable
    {
        public abstract bool IncludeSubdirectories { get; set; }
        public abstract bool EnableRaisingEvents { get; set; }
        public abstract string Filter { get; set; }
        public abstract int InternalBufferSize { get; set; }
        public abstract NotifyFilters NotifyFilter { get; set; }
        public abstract string Path { get; set; }
        public abstract ISite Site { get; set; }
        public virtual event FileSystemEventHandler Changed;
        public virtual event FileSystemEventHandler Created;
        public virtual event FileSystemEventHandler Deleted;
        public virtual event ErrorEventHandler Error;
        public virtual event RenamedEventHandler Renamed;
        public abstract void BeginInit();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void EndInit();
        public abstract WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType);
        public abstract WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout);

        public static implicit operator FileSystemWatcherBase(FileSystemWatcherShim watcher)
        {
            if (watcher == null)
            {
                throw new ArgumentNullException(nameof(watcher));
            }

            return new FileSystemWatcherWrapper(watcher);
        }

        protected virtual void Dispose(bool disposing)
        {
            // do nothing
        }

        protected void OnCreated(object sender, FileSystemEventArgs args)
        {
            FileSystemEventHandler onCreated = Created;
            onCreated?.Invoke(sender, args);
        }

        protected void OnChanged(object sender, FileSystemEventArgs args)
        {
            FileSystemEventHandler onChanged = Changed;
            onChanged?.Invoke(sender, args);
        }

        protected void OnDeleted(object sender, FileSystemEventArgs args)
        {
            FileSystemEventHandler onDeleted = Deleted;
            onDeleted?.Invoke(sender, args);
        }

        protected void OnRenamed(object sender, RenamedEventArgs args)
        {
            RenamedEventHandler onRenamed = Renamed;
            onRenamed?.Invoke(sender, args);
        }

        protected void OnError(object sender, ErrorEventArgs args)
        {
            ErrorEventHandler onError = Error;
            onError?.Invoke(sender, args);
        }
    }
}
