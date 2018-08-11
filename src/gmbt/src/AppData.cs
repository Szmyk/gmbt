using System;
using System.IO;

namespace GMBT
{
    /// <summary>
    /// Performs operations on tools' files located in %APPDATA%.
    /// </summary>
    public class AppData
    {
        public string Path { get; set; }
        public string Tools { get; set; }
        public string Logs { get; set; }
        public string Languages { get; set; }

        public AppData()
        {           
            Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GMBT");

            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }

            Tools = System.IO.Path.Combine(Path, "tools");
            Logs = System.IO.Path.Combine(Path, "logs");
            Languages = System.IO.Path.Combine(Path, "lang");
        }

        /// <summary>
        /// Returns absolute path tool given by name. If file is not exists, calls logger's fatal method and exit the program.
        /// </summary>
        /// <param name="fileName">File name (with extension) to search in `%AppData%/tools` directory</param>
        /// <example> 
        /// <code>
        /// string vdfsPath = GetTool("GothicVDFS.exe");
        /// </code>
        /// </example>
        public string GetTool(string fileName)
        {
            string path = System.IO.Path.Combine(Tools, fileName);

            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                Program.Logger.Fatal("GetTool.Error".Translate(path));
                return string.Empty;
            }
        }
    }
}
