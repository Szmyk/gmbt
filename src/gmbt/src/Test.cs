using System;
using System.IO;
using System.Linq;

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

        public void DetectIfWorldIsNotExists()
        {
            var worlds = Directory.GetFiles(gothic.GetGameDirectory(Gothic.GameDirectory.Worlds), "*.ZEN", SearchOption.AllDirectories).ToList();

            foreach(var dir in Program.Config.ModFiles.Assets)
            {
                var worldsDir = Path.Combine(dir, "Worlds");

                if (Directory.Exists(worldsDir))
                {
                    worlds.AddRange(Directory.GetFiles(worldsDir, "*.ZEN", SearchOption.AllDirectories).ToList());
                }              
            }

            var world = Program.Options.TestVerb.World ?? Program.Config.ModFiles.DefaultWorld;

            if (worlds.Where(x => Path.GetFileName(x) == Path.GetFileName(world)).Count() < 1)
            {
                Program.Logger.Fatal("Config.Error.FileDidNotFound".Translate(world));
            }         
        }

        /// <summary>
        /// Starts test.
        /// </summary>
        public override void Start()
        {
            DetectIfWorldIsNotExists();

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
                parameters.Add("ztexconvert");              
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
