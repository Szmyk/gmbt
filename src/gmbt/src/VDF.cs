using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Szmyk.Utils.Directory;

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

            directoriesToPack.Add(@"_work\Data\Anims\_compiled\*");
            directoriesToPack.Add(@"_work\Data\Meshes\_compiled\*");
            directoriesToPack.Add(@"_work\Data\Textures\_compiled\*");
            directoriesToPack.Add(@"_work\Data\Scripts\_compiled\*");
            directoriesToPack.Add(@"_work\Data\Scripts\Content\Cutscene\*");
            directoriesToPack.Add(@"_work\Data\Worlds\*");

            if (Program.Options.BuildVerb.NoPackSounds == false)
            {
                directoriesToPack.Add(@"_work\Data\Sound\*");
            }

            string vdfOutput = Program.Options.BuildVerb.Output ?? Program.Config.ModVdf.Output;

            VDFScript script = new VDFScript(gothic.GetGameDirectory(Gothic.GameDirectory.Root, false), vdfOutput, Program.Options.BuildVerb.Comment ?? Program.Config.ModVdf.Comment, directoriesToPack, Program.Config.ModVdf.Include, Program.Config.ModVdf.Exclude);

            builder.Arguments = "/B " + script.GenerateAndGetPath();

            Process.Start(builder).WaitForExit();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
