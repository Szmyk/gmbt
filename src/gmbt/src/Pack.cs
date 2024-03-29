﻿using System;

using Szmyk.Utils.Time;

namespace GMBT
{
    /// <summary>
    /// Performs the pack mode.
    /// </summary>
    internal class Pack : Mode
    {
        public Pack(Gothic gothic) : base(gothic) { }

        protected override void runHooks(HookType hookType, HookEvent hookEvent)
        {
            Program.HooksManager.RunHooks(HookMode.Common, hookType, hookEvent);
            Program.HooksManager.RunHooks(HookMode.Pack, hookType, hookEvent);
        }

        /// <summary>
        /// Starts build.
        /// </summary>
        public override void Start()
        {
            DateTime startTime = TimeHelper.Now;

            if (!Program.Options.PackVerb.SkipMerge)
            {
                runHooks(HookType.Pre, HookEvent.AssetsMerge);

                new Merge(gothic, Merge.MergeOptions.All).MergeAssets();

                runHooks(HookType.Post, HookEvent.AssetsMerge);
            }

            runHooks(HookType.Pre, HookEvent.VdfsPack);

            new VDF(gothic).RunBuilder();

            runHooks(HookType.Post, HookEvent.VdfsPack);

            Logger.Minimal("CompletedIn".Translate((TimeHelper.Now - startTime).Minutes, (TimeHelper.Now - startTime).Seconds));
        }

        public override GothicArguments GetGothicArguments() => new GothicArguments();
    }
}
