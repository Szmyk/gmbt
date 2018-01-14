using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Szmyk.Utils.Directory;

using I18NPortable;

namespace GMBT
{
    /// <summary> 
    /// Performs operations related to building VDF volume.
    /// </summary>
    internal class VDF
    {
        private readonly Gothic gothic;

        private readonly List<string> directoriesToPack = new List<string>();

        public VDF (Gothic gothic)
        {
            this.gothic = gothic;        
        }

        public void PreparePathsForMakingVDF()
        {
            new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF)).Delete();

            Console.Write("VDF.PreparingFiles".Translate() + " ");
            Program.Logger.Trace("VDF.PreparingFiles".Translate());

            foreach (string dir in Directory.EnumerateDirectories(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF) + dir.Replace(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), ""));

                directoriesToPack.Add((gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF) + dir.Replace(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), "")).Replace(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF), "_work\\Data\\") + "\\*");
            }

            List<string> extensions = new List<string>
            {
                ".asc", ".mds",
                ".mdl",".mdh", ".msb", ".man", ".mms", ".mrm", ".mmb",
                ".tex", ".fnt",
                ".dat", ".bin",
                ".zen"
            };

            if (Program.Options.BuildVerb.NoPackSounds == false)
            {
                extensions.Add(".wav");
            }

            foreach (var file in DirectoryUtils.GetFilesByExtensions(new DirectoryInfo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)), extensions.ToArray()))
            {
                if (file.LastWriteTime != Install.OriginalAssetsDateTime)
                {
                    file.CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF) + file.FullName.Replace(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), ""), true);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
      
        public void ClearDirectoriesBeforeMakingVDF()
        {
            new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataBak)).Delete();

            Directory.Move(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData),
                           gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataBak));

            Directory.Move(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF),
                           gothic.GetGameDirectory(Gothic.GameDirectory.WorkData));
        }

        public void ClearDirectoriesAfterMakingVDF()
        {
            Directory.Move(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData),
                           gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataToVDF));

            Directory.Move(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataBak),
                           gothic.GetGameDirectory(Gothic.GameDirectory.WorkData));
        }
          
        public void RunBuilder()
        {
            Console.Write("VDF.Building".Translate() + " ");
            Program.Logger.Trace("VDF.Building".Translate());

            if (Directory.Exists(Path.GetDirectoryName(Program.Config.ModVdf.Output)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Program.Config.ModVdf.Output));
            }

            ProcessStartInfo builder = new ProcessStartInfo
            {
                FileName = Program.AppData.GetTool("GothicVDFS.exe"),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string vdfOutput = Program.Options.BuildVerb.Output ?? Program.Config.ModVdf.Output;

            VDFScript script = new VDFScript(gothic.GetGameDirectory(Gothic.GameDirectory.Root), vdfOutput, Program.Config.ModVdf.Comment, directoriesToPack, Program.Config.ModVdf.Include, Program.Config.ModVdf.Exclude);

            builder.Arguments = "/B " + script.GenerateAndGetPath();

            Process.Start(builder).WaitForExit();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
