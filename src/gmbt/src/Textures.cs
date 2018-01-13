using System.IO;
using System.Diagnostics;

using Szmyk.Utils.Files;

using I18NPortable;

namespace GMBT
{
    /// <summary>
    /// Implements operations on textures
    /// </summary>
    internal static class Textures
    {
        public enum CompileMode
        {
            Normal,
            Quick
        }

        public static void CompileTextures(string inputDirectory, string outputDirectory)
        {           
            CompileTextures(inputDirectory, outputDirectory, SearchOption.AllDirectories);
        }

        public static void CompileTextures(string inputDirectory, string outputDirectory, SearchOption searchOption)
        {         
            int toCompile = 0;
            foreach (string file in Directory.GetFiles(inputDirectory, "*.tga", searchOption))
            {
                if (File.GetLastWriteTime(file) != Install.OriginalAssetsDateTime)
                {
                    toCompile++;
                }
            }

            if (toCompile == 0)
            {
                return;
            }

            Console.Write("CompilingTextures".Translate() + " ");
            Program.Logger.Trace("CompilingTextures".Translate());

            ProcessStartInfo tgaToDds = new ProcessStartInfo
            {
                FileName = Program.AppData.Tools + "nvdxt.exe",
            
                WorkingDirectory = outputDirectory,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            ProcessStartInfo dssToTex = new ProcessStartInfo
            {
                FileName = Program.AppData.Tools + "dds2ztex.exe",
                WorkingDirectory = outputDirectory,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (Program.Options.Common.TextureCompile == CompileMode.Quick)
            {
                tgaToDds.Arguments += " -quick";
            }

            using (ProgressBar bar = new ProgressBar(toCompile))
            {
                foreach (string file in Directory.GetFiles(inputDirectory, "*.tga", searchOption))
                {
                    if (File.GetLastWriteTime(file) != Install.OriginalAssetsDateTime)
                    {
                        tgaToDds.Arguments = "-file \"" + Path.GetFullPath(file) + "\"";
                        dssToTex.Arguments = "\"" + outputDirectory + "\\" + Path.GetFileName(Path.ChangeExtension(file, "dds")) + "\"";

                        Process.Start(tgaToDds).WaitForExit();
                        Process.Start(dssToTex).WaitForExit();

                        FileHelper.Delete(outputDirectory + "\\" + Path.GetFileName(Path.ChangeExtension(file, "dds")));

                        Program.Logger.Trace("\t" + file);

                        bar.Increase();
                    }
                }
            }             
        }
    }
}
