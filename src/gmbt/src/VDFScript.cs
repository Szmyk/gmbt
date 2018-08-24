using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GMBT
{
    /// <summary> 
    /// Reperesents a GothicVDFS builder script.
    /// </summary>
    public class VDFScript
    {
        private readonly List<string> directoriesToPack;

        private readonly string gothicRoot;
        private readonly string outputVdf;
        private readonly string comment;

        private readonly List<string> include;
        private readonly List<string> exclude;

        public VDFScript(string gothicRoot, string outputVdf, string comment, List<string> directoriesToPack, List<string> include, List<string> exclude)
        {
            this.gothicRoot = gothicRoot;
            this.outputVdf = outputVdf;
            this.comment = comment;
            this.directoriesToPack = directoriesToPack;
            this.include = include;
            this.exclude = exclude;
        }

        public string Generate ()
        {
            var sb = new StringBuilder();

            sb.AppendLine("[BEGINVDF]");
            sb.AppendLine("Comment=" + comment);
            sb.AppendLine("BaseDir=" + Path.GetFullPath(gothicRoot));

            Directory.CreateDirectory(Path.GetDirectoryName(outputVdf));

            sb.AppendLine("VDFName=" + Path.GetFullPath(outputVdf));
            sb.AppendLine("[FILES]");

            foreach (var dir in directoriesToPack)
            {
                sb.AppendLine(dir);
            }

            sb.AppendLine("[INCLUDE]");

            if (include != null)
            {
                foreach (var dir in include)
                {
                    sb.AppendLine(dir);
                }
            }

            sb.AppendLine("[EXCLUDE]");

            if (exclude != null)
            {
                foreach (var dir in exclude)
                {
                    sb.AppendLine(dir);
                }
            }

            sb.Append("[ENDVDF]");

            return sb.ToString();
        }

        public string GenerateAndGetPath()
        {
            string path = Path.GetTempFileName();

            var script = Generate();

            File.WriteAllText(path, script, Encoding.Default);       

            return path;
        }
    }
}
