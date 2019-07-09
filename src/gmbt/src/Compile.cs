using System;

using Szmyk.Utils.Time;

namespace GMBT
{
    internal enum CompileMode
    {
        Quick,
        Full
    }

    /// <summary>
    /// Performs the compile mode.
    /// </summary>
    internal class Compile : Mode
    {
        public CompileMode Mode { get; private set; }

        public Compile(Gothic gothic, CompileMode compileMode) : base(gothic)
        {
            Mode = compileMode;

            compilingAssetsWatcher.OnFileCompile = (compilingFile) =>
            {
                if (compilingFile.Contains("MENU.DAT"))
                {
                    gothic.EndProcess();
                }
            };
        }

        protected override void runHooks(HookType hookType, HookEvent hookEvent)
        {
            Program.HooksManager.RunHooks(HookMode.Common, hookType, hookEvent);
            Program.HooksManager.RunHooks(HookMode.Compile, hookType, hookEvent);

            if (Mode == CompileMode.Full)
            {
                Program.HooksManager.RunHooks(HookMode.FullCompile, hookType, hookEvent);
            }
            else if (Mode == CompileMode.Quick)
            {
                Program.HooksManager.RunHooks(HookMode.QuickCompile, hookType, hookEvent);
            }
        }

        /// <summary>
        /// Starts compilation.
        /// </summary>
        public override void Start()
        {
            DateTime startTime = TimeHelper.Now;

            if (Program.Options.CommonTestCompile.Merge != Merge.MergeOptions.None)
            {
                runHooks(HookType.Pre, HookEvent.AssetsMerge);

                Merge.MergeAssets(gothic, Program.Options.CommonTestCompile.Merge);

                runHooks(HookType.Post, HookEvent.AssetsMerge);
            }

            if ((Program.Options.CommonTestCompile.Merge == Merge.MergeOptions.All)
            || (Program.Options.CommonTestCompile.Merge.HasFlag(Merge.MergeOptions.Scripts)))
            {
                if (Program.Options.CommonTestBuildCompile.NoUpdateSubtitles == false)
                {
                    runHooks(HookType.Pre, HookEvent.SubtitlesUpdate);

                    UpdateDialogs();

                    runHooks(HookType.Post, HookEvent.SubtitlesUpdate);
                }
            }

            Logger.SetOnFatalEvent(() =>
            {
                gothic.Dispose();
            });

            if (Mode == CompileMode.Full)
            {
                gothic.DisableVdfs();
            }

            compilingAssetsWatcher.Start();

            ZSpy.Run();

            gothic.Start(GetGothicArguments()).WaitForExit();

            ZSpy.Abort();

            compilingAssetsWatcher.Stop();

            Logger.Minimal("CompletedIn".Translate((TimeHelper.Now - startTime).Minutes, (TimeHelper.Now - startTime).Seconds));
        }

        /// <summary>
        /// Returns arguments list that will be passed to game process.
        /// </summary>
        public override GothicArguments GetGothicArguments()
        {
            GothicArguments parameters = new GothicArguments();

            parameters.Add("zreparse");

            parameters.Add("zwindow");

            parameters.Add("vdfs", "physicalfirst");

            parameters.Add("3d", "none");

            parameters.Add("nomenu");

            if (Mode == CompileMode.Full)
            {             
                parameters.Add("zconvertall");
                parameters.Add("ztexconvert");               
                parameters.Add("zautoconvertdata");
            }

            return parameters;
        }
    }
}
