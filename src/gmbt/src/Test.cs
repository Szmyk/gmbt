using System;

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

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Done".Translate());
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            };
        }  

        /// <summary>
        /// Starts test.
        /// </summary>
        public override void Start()
        {
            if (Program.Options.TestVerb.Merge != Merge.MergeOptions.None)
            {
                Merge.MergeAssets(gothic, Program.Options.TestVerb.Merge);
            }

            if (Program.Options.CommonTestBuild.NoUpdateSubtitles == false)
            {
                Console.Write("ConvertingSubtitles".Translate() + " ");
                Program.Logger.Trace("ConvertingSubtitles".Translate());

                OutputUnitsUpdater.OutputUnitsUpdater.Update(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsContent),
                                                             gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCutscene) + "OU.csl");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done".Translate());
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            if (Mode == TestMode.Full)
            {
                Textures.CompileTextures(gothic.GetGameDirectory(Gothic.GameDirectory.Textures),
                                         gothic.GetGameDirectory(Gothic.GameDirectory.TexturesCompiled));
            }
          
            if (Program.Options.TestVerb.ZSpyLevel != ZSpy.Mode.None)
            {
                ZSpy.Run();
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

            if (Program.Options.TestVerb.ZSpyLevel != ZSpy.Mode.None)
            {
                parameters.Add("zlog", Convert.ToInt32(Program.Options.TestVerb.ZSpyLevel) + ",s");
            }

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

            return parameters;
        }
    }
}
