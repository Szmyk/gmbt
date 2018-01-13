using System;

using Szmyk.Utils.Time;

using I18NPortable;

namespace GMBT
{   
    /// <summary>
    /// Performs the build mode.
    /// </summary>
    internal class Build : Mode
    {       
        public Build (Gothic gothic) : base(gothic)
        {
            compilingAssetsWatcher.OnFileCompile = (compilingFile) =>
            {
                if (compilingFile.Contains("CAMERA.DAT"))
                {
                    gothic.EndProcess();
                }
            };    
        }

        /// <summary>
        /// Starts build.
        /// </summary>
        public override void Start()
        {
            DateTime startTime = TimeHelper.Now;

            Merge.MergeAssets(gothic, Merge.MergeOptions.All);

            if (Program.Options.Common.NoUpdateSubtitles == false)
            {            
                Console.Write("ConvertingSubtitles".Translate() + " ");
                Program.Logger.Trace("ConvertingSubtitles".Translate());

                OutputUnitsUpdater.OutputUnitsUpdater.Update(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsContent),
                                                             gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCutscene) + "OU.csl");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done".Translate());
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Textures.CompileTextures(gothic.GetGameDirectory(Gothic.GameDirectory.Textures),
                                     gothic.GetGameDirectory(Gothic.GameDirectory.TexturesCompiled));

            compilingAssetsWatcher.Start();

            gothic.Start(GetGothicArguments()).WaitForExit();

            compilingAssetsWatcher.Stop();

            VDF vdf = new VDF(gothic);
            vdf.PreparePathsForMakingVDF();
            vdf.ClearDirectoriesBeforeMakingVDF();
            vdf.RunBuilder();
            vdf.ClearDirectoriesAfterMakingVDF();

            Program.Logger.Info(string.Format("CompletedIn".Translate(), ( TimeHelper.Now - startTime ).Minutes, ( TimeHelper.Now - startTime ).Seconds));
        }

        /// <summary>
        /// Returns arguments list that will be passed to game process.
        /// </summary>
        public override GothicArguments GetGothicArguments()
        {
            GothicArguments arguments = new GothicArguments();

            arguments.Add("zwindow");
            arguments.Add("zreparse");
            arguments.Add("zconvertall");
            arguments.Add("nomenu");
            arguments.Add("3d", "none");

            return arguments;
        }
    }
}
