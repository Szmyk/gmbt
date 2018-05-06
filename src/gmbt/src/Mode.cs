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
    }
}
