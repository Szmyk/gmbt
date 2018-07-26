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

        protected override void runHooks(HookType hookType, HookEvent hookEvent)
        {
            Program.HooksManager.RunHooks(HookMode.Common, hookType, hookEvent);
            Program.HooksManager.RunHooks(HookMode.Build, hookType, hookEvent);        
        }

        /// <summary>
        /// Starts build.
        /// </summary>
        public override void Start()
        {
            DateTime startTime = TimeHelper.Now;

            runHooks(HookType.Pre, HookEvent.AssetsMerge);

            Merge.MergeAssets(gothic, Merge.MergeOptions.All);

            runHooks(HookType.Post, HookEvent.AssetsMerge);

            if (Program.Options.CommonTestBuild.NoUpdateSubtitles == false)
            {
                runHooks(HookType.Pre, HookEvent.SubtitlesUpdate);

                UpdateDialogs();

                runHooks(HookType.Post, HookEvent.SubtitlesUpdate);
            }

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
            arguments.Add("ztexconvert");
            arguments.Add("nomenu");
            arguments.Add("3d", "none");

            return arguments;
        }
    }
}
