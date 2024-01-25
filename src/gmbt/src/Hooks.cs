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
        Common, Test, QuickTest, FullTest, Build, Pack, Compile, QuickCompile, FullCompile
    }

    internal enum HookEvent
    {
        AssetsMerge,
        SubtitlesUpdate,
        VdfsPack
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
                var hookParameters = Program.Options.CommonTestSpacerBuildPackCompile.HooksForwardParameter ?? string.Empty;

                var processStartInfo = new ProcessStartInfo(Command);
                var process = new Process();
                process.StartInfo = processStartInfo;
                process.StartInfo.Arguments = hookParameters;

                Logger.Normal("Hooks.Run.WaitingForEnd".Translate());

                if (Logger.Verbosity >= VerbosityLevel.Detailed)
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.OutputDataReceived += (object sender, DataReceivedEventArgs args) =>
                    {
                        if (args.Data != null)
                        {
                            Logger.Detailed(args.Data);
                        }
                    };
                    process.ErrorDataReceived += (object sender, DataReceivedEventArgs args) =>
                    {
                        if (args.Data != null)
                        {
                            Logger.Error(args.Data);
                        }
                    };
                }

                process.Start();

                if (Logger.Verbosity >= VerbosityLevel.Detailed)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    if (Program.Options.CommonTestSpacerBuildPackCompile.HooksIgnoreFailures)
                    {
                        Logger.Warn("Hooks.Run.ErrorCode".Translate(process.ExitCode));
                    }
                    else
                    {
                        Logger.Fatal("Hooks.Run.ErrorCode".Translate(process.ExitCode));
                    }
                } 
            }
            catch (Exception ex)
            {
                Logger.Warn("Hooks.Run.Error".Translate(ex.Message));
            }
        }

        public override string ToString()
        {
            if (Program.Options.CommonTestSpacerBuildPackCompile.HooksForwardParameter != null)
            {
                return $"[{Mode}] [{Type}] [{Event}] [{Command}] [{Program.Options.CommonTestSpacerBuildPackCompile.HooksForwardParameter}]";
            }
            else
            {
                return $"[{Mode}] [{Type}] [{Event}] [{Command}]";
            }
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
            if (Program.Options.CommonTestSpacerBuildPackCompile.NoHooks == false)
            {
                Hooks.Where(hook => hook.Mode == hookMode)
                .Where(hook => hook.Event == hookEvent)
                .Where(hook => hook.Type == hookType).ToList()
                .ForEach(hook => hook.Run());
            }         
        }
    }
}
