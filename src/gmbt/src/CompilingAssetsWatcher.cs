using System;
using System.IO;

namespace GMBT
{
    /// <summary>
    /// Performs watching of currently compiling assets by game.
    /// </summary>
    internal class CompilingAssetsWatcher : FileSystemWatcher
    {
        public Action<string> OnFileCompile { get; set; }
      
        public CompilingAssetsWatcher(string path, string filter) : base(path, filter)
        {
            NotifyFilter = NotifyFilters.LastWrite;

            IncludeSubdirectories = true;

            Changed += OnChanged;
        }

        string lastCompiledFile;

        void OnChanged(object source, FileSystemEventArgs e)
        {
            if (File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            if (e.Name != lastCompiledFile)
            {
                Logger.Detailed("\t" + "Compiled".Translate() + ": " + e.Name);

                OnFileCompile?.Invoke(e.FullPath.ToUpper());

                lastCompiledFile = e.Name;
            }
        }

        public void Start() => EnableRaisingEvents = true;

        public void Stop() => EnableRaisingEvents = false;
    }
}
