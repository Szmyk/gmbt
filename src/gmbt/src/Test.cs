using System;
using System.IO;

namespace GMBT
{
    internal enum TestMode
    {
        Quick,
        Full
    }

    /// <summary>
    /// Implements the test mode.
    /// </summary>
    internal class Test : Mode
    {
        public TestMode Mode { get; private set; }

        private bool assetsCompiled;

        public Test(Gothic gothic, TestMode testMode) : base(gothic)
        {
            Mode = testMode;

            compilingAssetsWatcher.OnFileCompile = (compilingFile) =>
            {              
                if (compilingFile.Contains("MENU.DAT"))
                {
                    if ((Mode == TestMode.Full
                    && (assetsCompiled == false)))
                    {
                        gothic.EndProcess();
                    }

                    Console.WriteLine("Done".Translate(), ConsoleColor.Green);
                }
            };
        }

        protected override void runHooks(HookType hookType, HookEvent hookEvent)
        {
            Program.HooksManager.RunHooks(HookMode.Common, hookType, hookEvent);
            Program.HooksManager.RunHooks(HookMode.Test, hookType, hookEvent);

            if (Mode == TestMode.Full)
            {
                Program.HooksManager.RunHooks(HookMode.FullTest, hookType, hookEvent);
            }
            else if (Mode == TestMode.Quick)
            {
                Program.HooksManager.RunHooks(HookMode.QuickTest, hookType, hookEvent);
            }
        }

        /// <summary>
        /// Starts test.
        /// </summary>
        public override void Start()
        {
            if (Program.Options.TestVerb.Merge != Merge.MergeOptions.None)
            {
                runHooks(HookType.Pre, HookEvent.AssetsMerge);

                Merge.MergeAssets(gothic, Program.Options.TestVerb.Merge);

                runHooks(HookType.Post, HookEvent.AssetsMerge);
            }

            if (Program.Options.CommonTestBuild.NoUpdateSubtitles == false)
            {
                runHooks(HookType.Pre, HookEvent.SubtitlesUpdate);

                UpdateDialogs();

                runHooks(HookType.Post, HookEvent.SubtitlesUpdate);
            }

            if (Mode == TestMode.Full)
            {
                runHooks(HookType.Pre, HookEvent.TexturesCompile);

                Textures.CompileTextures(gothic.GetGameDirectory(Gothic.GameDirectory.Textures),
                                         gothic.GetGameDirectory(Gothic.GameDirectory.TexturesCompiled));

                runHooks(HookType.Post, HookEvent.TexturesCompile);
            }        

            compilingAssetsWatcher.Start();

            gothic.Start(GetGothicArguments()).WaitForExit();

            assetsCompiled = true;

            if (Mode == TestMode.Full)
            {
                gothic.Start(GetGothicArguments()).WaitForExit();             
            }

            compilingAssetsWatcher.Stop();
        }

        /// <summary>
        /// Returns arguments list that will be passed to game process.
        /// </summary>
        public override GothicArguments GetGothicArguments()
        {
            GothicArguments parameters = new GothicArguments();

            parameters.Add("zreparse");

            if (Program.Options.TestVerb.RunGothicWindowed
            || (Mode == TestMode.Full && assetsCompiled == false))
            {
                parameters.Add("zwindow"); 
            }

            if (Program.Options.TestVerb.InGameTime != null)
            {
                parameters.Add("time", Program.Options.TestVerb.InGameTime);
            }

            if (Mode == TestMode.Full && assetsCompiled == false)
            {
                parameters.Add("3d", "none");
                parameters.Add("zconvertall");
                parameters.Add("nomenu");
            }
            else
            {
                parameters.Add("3d", Program.Options.TestVerb.World ?? Program.Config.ModFiles.DefaultWorld);
            }

            if (Program.Options.TestVerb.NoAudio)
            {
                if (File.Exists(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCompiled) + "MUSIC.DAT"))
                {
                    parameters.Add("znomusic");
                }

                if (File.Exists(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCompiled) + "SFX.DAT"))
                {
                    parameters.Add("znosound");
                }
            }

            if (Program.Options.TestVerb.NoMenu)
            {
                parameters.Add("nomenu");
            }

            return parameters;
        }
    }
}
