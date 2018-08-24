using System.IO;

namespace GMBT
{
    /// <summary> 
    /// Represents mode of currently tool instance. 
    /// </summary>
    internal abstract class Mode
    {
        protected readonly Gothic gothic;

        protected readonly CompilingAssetsWatcher compilingAssetsWatcher;

        protected Mode(Gothic gothic)
        {
            this.gothic = gothic;

            compilingAssetsWatcher = new CompilingAssetsWatcher(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), "*.*");
        }

        public abstract GothicArguments GetGothicArguments();

        public abstract void Start();

        protected abstract void runHooks(HookType hookType, HookEvent hookEvent);

        protected void UpdateDialogs()
        {
            var gothicSrc = gothic.GetGameFile(Gothic.GameFile.GothicSrc);

            if (File.Exists(gothicSrc) == false)
            {
                Logger.Fatal("Config.Error.FileDidNotFound".Translate(gothicSrc));
            }

            Logger.Minimal("ConvertingSubtitles".Translate());

            var updater = new OutputUnitsUpdater.OutputUnitsUpdater
            (
                gothicSrc,
                Path.Combine(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCutscene), "OU.csl")
            );

            updater.Update();

            if (Program.Options.CommonTestBuild.ShowDuplicatedSubtitles)
            {
                Logger.Normal("SearchingDuplicatedSubtitles".Translate());

                var duplicated = updater.GetDuplicates();

                if (duplicated.Count > 0)
                {
                    Logger.Warn("FoundDuplicatedDialogs".Translate());

                    foreach (var duplicate in duplicated)
                    {
                        Logger.Warn("\t" + duplicate.Name);
                    }
                }
            }         
        }
    }
}
