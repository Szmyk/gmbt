using System.IO;
using System.Diagnostics;

using Szmyk.Utils.File;

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

            if (Program.Options.CommonTestBuild.TextureCompile == CompileMode.Quick)
            {
                tgaToDds.Arguments += " -quick";
            }

            Program.Logger.Trace("CompilingTextures".Translate());

            using (ProgressBar bar = new ProgressBar("CompilingTextures".Translate(), toCompile))
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
