using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Szmyk.Utils.INI;
using Szmyk.Utils.Paths;

namespace GMBT
{ 
    /// <summary>
    /// Performs operations on Gothic or Gothic 2 Night of the Raven process.
    /// </summary>
    internal class Gothic : IDisposable
    {
        public enum GameVersion
        {
            Gothic1,
            Gothic2
        }

        public GameVersion Version { get; private set; }

        public IniFile GothicINI { get; private set; }

        private readonly string rootDirectory;
        private readonly string exeFile;

        public string GetGothicVersionName ()
        {
            return Version == GameVersion.Gothic1 
                            ? "Gothic" 
                            : "Gothic2NotR".Translate();
        }

        public Gothic(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;

            if (File.Exists(GetGameDirectory(GameDirectory.System) + "Gothic2.exe"))
            {
                exeFile = GetGameDirectory(GameDirectory.System) + "Gothic2.exe";

                Version = GameVersion.Gothic2;
            }
            else if (File.Exists(GetGameDirectory(GameDirectory.System) + "Gothic.exe"))
            {
                exeFile = GetGameDirectory(GameDirectory.System) + "Gothic.exe";

                Version = GameVersion.Gothic1;
            }
            else
            {
                Program.Logger.Fatal("Gothic.Error.DidNotFoundExe".Translate());
            }

            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeFile)).Length > 0)
            {
                Program.Logger.Fatal("Gothic.Error.AlreadyRunning".Translate(GetGothicVersionName()));
            }

            GothicINI = new IniFile(GetGameDirectory(GameDirectory.System) + "GOTHIC.INI");           
        }

        public void Dispose()
        {
            EndProcess();
        }

        private Process gothicProcess;

        public void EndProcess ()
        {           
            if (gothicProcess?.HasExited == false)
            {
                gothicProcess.Kill();
            }      
        }

        private void overrideGothicIniKeys()
        {
            var gmbtIniPath = GetGameDirectory(GameDirectory.System) + "gmbt.ini";

            if (File.Exists(gmbtIniPath) == false)
            {
                File.Create(gmbtIniPath);
            }

            var gmbtIni = new IniFile(gmbtIniPath);

            if (Program.Config.GothicIniOverrides != null)
            {
                foreach (var dictionary in Program.Config.GothicIniOverrides)
                {
                    foreach (var tuple in dictionary)
                    {                     
                        gmbtIni.Write(tuple.Key, tuple.Value, "OVERRIDES");
                    }
                }
            }
        }

        public Process Start (GothicArguments arguments)
        {
            if (Program.Options.CommonTestBuild.ZSpyLevel != ZSpy.Mode.None)
            {
                ZSpy.Run();
            }

            overrideGothicIniKeys();

            createDirectoriesForCompiledAssets();

            ProcessStartInfo gothic = new ProcessStartInfo
            {
                FileName = Version == GameVersion.Gothic1
                         ? GetGameDirectory(GameDirectory.System) + "Gothic.exe"
                         : GetGameDirectory(GameDirectory.System) + "Gothic2.exe",
            };

            gothic.Arguments = GetCommonParameters().ToString() + arguments.ToString();
            
            Program.Logger.Trace("Gothic.RunningWithParameters".Translate(GetGothicVersionName(), gothic.Arguments));

            using (ProgressBar bar = new ProgressBar("Gothic.Running".Translate(GetGothicVersionName()), 1))
            {
                OnOffDirectX11Wrapper(Program.Options.TestVerb.NoDirectX11);

                gothicProcess = new Process();
                gothicProcess.StartInfo = gothic;

                gothicProcess.Start();

                bar.Increase();
            }

            Console.Write("Gothic.CompilingAssets".Translate() + (Program.Options.CommonTestBuild.ShowCompilingAssets ? "\n" : " "));

            return gothicProcess;
        }
       
        public void OnOffDirectX11Wrapper (bool off)
        {
            if (File.Exists(GetGameDirectory(GameDirectory.System) + "ddraw.dll")
            && Directory.Exists(GetGameDirectory(GameDirectory.GD3D11)))
            {
                string dllPath = GetGameDirectory(GameDirectory.System) + "ddraw.dll";

                string extension = off 
                                 ? ".off" 
                                 : ".dll";

                File.Move(dllPath, PathsUtils.ChangeExtension(dllPath, extension));
            }
        }
     
        public GothicArguments GetCommonParameters ()
        {
            GothicArguments arguments = new GothicArguments();

            if (Program.Options.CommonTestBuild.ZSpyLevel != ZSpy.Mode.None)
            {
                arguments.Add("zlog", Convert.ToInt32(Program.Options.CommonTestBuild.ZSpyLevel) + ",s");
            }

            arguments.Add("game", "gmbt.ini");         

            return arguments;
        }

        public enum GameDirectory
        {
            Root,
            System,
            Data, ModVDF,
            Work, WorkData, WorkDataOrg, WorkDataToVDF, WorkDataBak,
            Scripts, ScriptsCompiled, ScriptsCutscene, ScriptsContent,
            Anims, AnimsCompiled,
            Meshes, MeshesCompiled,
            Textures, TexturesCompiled,
            Sound, Worlds,
            Video, Music, Presets,
            GD3D11
        }

        public string GetGameDirectory(GameDirectory dir, bool endSlashes)
        {
            string end = endSlashes 
                       ? "\\"
                       : string.Empty;

            switch (dir)
            {
                case GameDirectory.Root:             return Path.GetFullPath(rootDirectory) + end;           
                case GameDirectory.System:           return GetGameDirectory(GameDirectory.Root) + "System" + end;
                case GameDirectory.GD3D11:           return GetGameDirectory(GameDirectory.System) + "GD3D11" + end;
                case GameDirectory.Data:             return GetGameDirectory(GameDirectory.Root) + "Data" + end;
                case GameDirectory.ModVDF:           return GetGameDirectory(GameDirectory.Data) + "ModVDF" + end;
                case GameDirectory.Work:             return GetGameDirectory(GameDirectory.Root) + "_Work" + end;
                case GameDirectory.WorkData:         return GetGameDirectory(GameDirectory.Work) + "Data" + end;
                case GameDirectory.WorkDataOrg:      return GetGameDirectory(GameDirectory.Work) + "DataOriginal" + end;
                case GameDirectory.WorkDataToVDF:    return GetGameDirectory(GameDirectory.Work) + "DataToVDF" + end;
                case GameDirectory.WorkDataBak:      return GetGameDirectory(GameDirectory.Work) + "DataBak" + end;
                case GameDirectory.ScriptsCompiled:  return GetGameDirectory(GameDirectory.Scripts) + "_compiled" + end;
                case GameDirectory.ScriptsCutscene:  return GetGameDirectory(GameDirectory.ScriptsContent) + "Cutscene" + end;
                case GameDirectory.ScriptsContent:   return GetGameDirectory(GameDirectory.Scripts) + "Content" + end;
                case GameDirectory.AnimsCompiled:    return GetGameDirectory(GameDirectory.Anims) + "_compiled" + end;
                case GameDirectory.MeshesCompiled:   return GetGameDirectory(GameDirectory.Meshes) + "_compiled" + end;
                case GameDirectory.TexturesCompiled: return GetGameDirectory(GameDirectory.Textures) + "_compiled" + end;
                default:                             return GetGameDirectory(GameDirectory.WorkData) + Enum.GetName(typeof(GameDirectory), dir) + end;
            }        
        }

        public string GetGameDirectory(GameDirectory dir)
        {
            return GetGameDirectory(dir, true);
        }

        public DirectoryInfo GetGameDirectoryInfo(GameDirectory dir)
        {
            return new DirectoryInfo(GetGameDirectory(dir));
        }

        public DirectoryInfo GetGameDirectoryInfo(GameDirectory dir, bool endSlashes)
        {
            return new DirectoryInfo(GetGameDirectory(dir, endSlashes));
        }

        private void createDirectoriesForCompiledAssets()
        {
            foreach (GameDirectory dir in Enum.GetValues(typeof(GameDirectory)))
            {
                Directory.CreateDirectory(GetGameDirectory(dir));
            }
        }
    }
}
