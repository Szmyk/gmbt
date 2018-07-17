using System;
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
            Program.Logger.Info("ConvertingSubtitles".Translate());

            var updater = new OutputUnitsUpdater.OutputUnitsUpdater
            (
                Path.Combine(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsContent), "Gothic.src"),
                Path.Combine(gothic.GetGameDirectory(Gothic.GameDirectory.ScriptsCutscene), "OU.csl")
            );

            updater.Update();

            var duplicated = updater.GetDuplicates();

            if (duplicated.Count > 0)
            {
                Program.Logger.Error("FoundDuplicatedDialogs".Translate());

                foreach (var duplicate in duplicated)
                {
                    Program.Logger.Error("\t" + duplicate.Name);
                }
            }
        }
    }
}
