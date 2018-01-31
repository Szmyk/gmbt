using System;

using Szmyk.Utils.Time;

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

            if (Program.Options.CommonTestBuild.NoUpdateSubtitles == false)
            {
                var message = "ConvertingSubtitles".Translate();

                Program.Logger.Trace(message);

                using (ProgressBar bar = new ProgressBar(message, 1))
                {
                    OutputUnitsUpdater.OutputUnitsUpdater.Update(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsContent),
                                                                 gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCutscene) + "OU.csl");
                }
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

            Program.Logger.Info("CompletedIn".Translate((TimeHelper.Now - startTime).Minutes, (TimeHelper.Now - startTime).Seconds));
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
