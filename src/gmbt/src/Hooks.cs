using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace GMBT
{
    using HooksTree = Dictionary<HookMode,
                      Dictionary<HookType,
                      Dictionary<HookEvent, List<string>>>>;

    internal enum HookType
    {
        Pre, Post
    }

    internal enum HookMode
    {
        Common, Test, QuickTest, FullTest, Build
    }

    internal enum HookEvent
    {
        AssetsMerge,
        SubtitlesUpdate
    }

    internal class Hook
    {
        public HookType Type { get; set; }
        public HookMode Mode { get; set; }
        public HookEvent Event { get; set; }
        public string Command { get; set; }

        public void Run()
        {
            Logger.Normal("Hooks.Run".Translate(this));

            try
            {
                var process = Process.Start(Command);

                Logger.Normal("Hooks.Run.WaitingForEnd".Translate());

                process.WaitForExit();           
            }
            catch (Exception ex)
            {
                Logger.Warn("Hooks.Run.Error".Translate(ex.Message));
            }
        }

        public override string ToString()
        {
            return $"[{Mode}] [{Type}] [{Event}] [{Command}]";
        }

        public override bool Equals(Object obj)
        {
            if (obj == null 
            || GetType() != obj.GetType())
            {
                return false;
            }
              
            Hook hook = obj as Hook;

            return Type == hook.Type 
                && Mode == hook.Mode
                && Event == hook.Event
                && Command == hook.Command;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    internal class HooksManager
    {
        public List<Hook> Hooks = new List<Hook>();

        public void RegisterHooks (HooksTree hooks)
        {           
            foreach (var modes in hooks)
            {
                foreach (var types in modes.Value)
                {
                    foreach (var events in types.Value)
                    {
                        foreach (var command in events.Value)
                        {
                            Hooks.Add(new Hook
                            {
                                Mode = modes.Key,
                                Type = types.Key,
                                Event = events.Key,
                                Command = command
                            });
                        }
                    }
                }
            }      
        }

        public void RunHooks(HookMode hookMode, HookType hookType, HookEvent hookEvent)
        {
            Hooks.Where(hook => hook.Mode == hookMode)
                 .Where(hook => hook.Event == hookEvent)
                 .Where(hook => hook.Type == hookType).ToList()
                 .ForEach(hook => hook.Run());
        }
    }
}
