using System.ComponentModel;

namespace System.IO.Abstractions
{
    [Serializable]
    public class FileSystemWatcherWrapper : FileSystemWatcherBase
    {
        [NonSerialized]
        private readonly FileSystemWatcherShim _watcher;

        public FileSystemWatcherWrapper()
            : this(new FileSystemWatcherShim())
        {
            // do nothing
        }

        public FileSystemWatcherWrapper(string path)
            : this(new FileSystemWatcherShim(path))
        {
            // do nothing
        }

        public FileSystemWatcherWrapper(string path, string filter)
            : this(new FileSystemWatcherShim(path, filter))
        {
            // do nothing
        }

        public FileSystemWatcherWrapper(FileSystemWatcherShim watcher)
        {
            if (watcher == null)
            {
                throw new ArgumentNullException(nameof(watcher));
            }

            this._watcher = watcher;
            this._watcher.Created += OnCreated;
            this._watcher.Changed += OnChanged;
            this._watcher.Deleted += OnDeleted;
            this._watcher.Error += OnError;
            this._watcher.Renamed += OnRenamed;
        }

        public override bool IncludeSubdirectories
        {
            get { return _watcher.IncludeSubdirectories; }
            set { _watcher.IncludeSubdirectories = value; }
        }

        public override bool EnableRaisingEvents
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }

        public override string Filter
        {
            get { return _watcher.Filter; }
            set { _watcher.Filter = value; }
        }

        public override int InternalBufferSize
        {
            get { return _watcher.InternalBufferSize; }
            set { _watcher.InternalBufferSize = value; }
        }

        public override NotifyFilters NotifyFilter
        {
            get { return _watcher.NotifyFilter; }
            set { _watcher.NotifyFilter = value; }
        }

        public override string Path
        {
            get { return _watcher.Path; }
            set { _watcher.Path = value; }
        }

        public override ISite Site
        {
            get { return _watcher.Site; }
            set { _watcher.Site = value; }
        }

        public override void BeginInit()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher.Created -= OnCreated;
                _watcher.Changed -= OnChanged;
                _watcher.Deleted -= OnDeleted;
                _watcher.Error -= OnError;
                _watcher.Renamed -= OnRenamed;
                _watcher.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void EndInit()
        {
        }

        public override WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType) => _watcher.WaitForChanged(changeType);

        public override WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout) => _watcher.WaitForChanged(changeType, timeout);
    }
}
