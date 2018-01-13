using System;
using System.IO;

using I18NPortable;

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

        public AppData()
        {           
            Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//GMBT//";

            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }

            Tools = Path + "tools//";
            Logs = Path + "logs//";           
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
            string path = Tools + fileName;

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
