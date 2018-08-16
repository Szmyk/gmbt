using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Szmyk.Utils.INI;
using Szmyk.Utils.Paths;

using VdfsSharp;

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

            if (File.Exists(GetGameFile(GameFile.Gothic2Exe)))
            {
                exeFile = GetGameFile(GameFile.Gothic2Exe);

                Version = GameVersion.Gothic2;
            }
            else if (File.Exists(GetGameFile(GameFile.Gothic1Exe)))
            {
                exeFile = GetGameFile(GameFile.Gothic1Exe);

                Version = GameVersion.Gothic1;
            }
            else
            {
                Logger.Fatal("Gothic.Error.DidNotFoundExe".Translate());
            }

            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeFile)).Length > 0)
            {
                Logger.Fatal("Gothic.Error.AlreadyRunning".Translate(GetGothicVersionName()));
            }

            GothicINI = new IniFile(GetGameFile(GameFile.GothicIni));

            Logger.SetOnFatalEvent(() =>
            {
                EnableVdfs();
                gothicProcess?.Kill();
            });
        }

        List<string> disabledVdfs = new List<string>();

        public void EnableVdfs()
        {
            foreach (var vdf in disabledVdfs)
            {
                if (File.Exists(vdf))
                {
                    File.Move(vdf, PathsUtils.ChangeExtension(vdf, ".vdf"));
                }     
            }
        }

        public void DisableVdfs()
        {
            foreach (var vdf in Directory.GetFiles(GetGameDirectory(Gothic.GameDirectory.Data)))
            {
                var reader = new VdfsReader(vdf);

                var hasAnims = reader
                    .ReadEntries(false)
                    .Where(x => x.Name.Equals("ANIMS", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Name).Count() > 0;

                reader.Dispose();

                if (hasAnims)
                {
                    var newPath = PathsUtils.ChangeExtension(vdf, ".disabled");

                    disabledVdfs.Add(newPath);

                    File.Move(vdf, newPath);
                }
            }
        }

        public void Dispose()
        {
            EnableVdfs();

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
            if (Program.Config.GothicIniOverrides != null)
            {               
                foreach (var dictionary in Program.Config.GothicIniOverrides)
                {
                    foreach (var tuple in dictionary)
                    {
                        var split = tuple.Key.Split('.');

                        GothicINI.Write(split[1], tuple.Value, split[0]);
                    }
                }
            }
        }

        public Process Start (GothicArguments arguments)
        {
            overrideGothicIniKeys();

            createDirectoriesForCompiledAssets();

            ProcessStartInfo gothic = new ProcessStartInfo
            {
                FileName = Version == GameVersion.Gothic1
                         ? GetGameFile(GameFile.Gothic1Exe)
                         : GetGameFile(GameFile.Gothic2Exe),

                WorkingDirectory = GetGameDirectory(GameDirectory.System)
            };

            gothic.Arguments = GetCommonParameters().ToString() + arguments.ToString();

            Logger.Detailed("Gothic.RunningWithParameters".Translate(GetGothicVersionName(), gothic.Arguments));

            if (Logger.Verbosity <= VerbosityLevel.Detailed)
            {
                Logger.Minimal("Gothic.Running".Translate(GetGothicVersionName()));
            }

            OnOffDirectX11Wrapper(Program.Options.TestVerb.NoDirectX11);

            gothicProcess = new Process();
            gothicProcess.StartInfo = gothic;

            gothicProcess.Start();

            Logger.Minimal("Gothic.CompilingAssets".Translate());

            return gothicProcess;
        }
       
        public void OnOffDirectX11Wrapper (bool off)
        {
            string dllPath = GetGameFile(GameFile.DdrawDll);

            if (File.Exists(dllPath)
            && Directory.Exists(GetGameDirectory(GameDirectory.GD3D11)))
            {              
                string extension = off 
                                 ? ".off" 
                                 : ".dll";

                File.Move(dllPath, PathsUtils.ChangeExtension(dllPath, extension));
            }
        }
     
        public GothicArguments GetCommonParameters ()
        {
            GothicArguments arguments = new GothicArguments();

            if (Program.Options.CommonTestSpacerBuild.ZSpyLevel != ZSpy.Mode.None)
            {
                arguments.Add("zlog", Convert.ToInt32(Program.Options.CommonTestSpacerBuild.ZSpyLevel) + ",s");
            }

            arguments.Add("ini", Path.GetFileName(GetGameFile(GameFile.GothicIni)));         

            return arguments;
        }

        public enum GameDirectory
        {
            Root,
            System,
            Data, ModVDF,
            Work, WorkData, WorkDataOrg,
            Scripts, ScriptsCompiled, ScriptsCutscene, ScriptsContent,
            Anims, AnimsCompiled,
            Meshes, MeshesCompiled,
            Textures, TexturesCompiled,
            Sound, Worlds,
            Video, Music, Presets,
            GD3D11
        }

        public enum GameFile
        {
            GothicDat, MusicDat, SfxDat,
            GothicSrc,
            OuCsl,
            WorldsVdf, WorldsAddonVdf,
            Gothic1Exe, Gothic2Exe,
            SpacerExe, Spacer2Exe,
            GothicIni,
            DdrawDll
        }

        public string GetGameFile (GameFile file)
        {
            switch (file)
            { 
                case GameFile.GothicDat:      return Path.Combine(GetGameDirectory(GameDirectory.ScriptsCompiled), "GOTHIC.DAT");
                case GameFile.MusicDat:       return Path.Combine(GetGameDirectory(GameDirectory.ScriptsCompiled), "MUSIC.DAT");
                case GameFile.SfxDat:         return Path.Combine(GetGameDirectory(GameDirectory.ScriptsCompiled), "SFX.DAT");
                case GameFile.GothicSrc:      return Path.Combine(GetGameDirectory(GameDirectory.ScriptsContent), "Gothic.src");
                case GameFile.OuCsl:          return Path.Combine(GetGameDirectory(GameDirectory.ScriptsCutscene), "OU.CSL");
                case GameFile.WorldsVdf:      return Path.Combine(GetGameDirectory(GameDirectory.Data), "Worlds.vdf");
                case GameFile.WorldsAddonVdf: return Path.Combine(GetGameDirectory(GameDirectory.Data), "Worlds_Addon.vdf");
                case GameFile.Gothic1Exe:     return Path.Combine(GetGameDirectory(GameDirectory.System), "GothicMod.exe");
                case GameFile.Gothic2Exe:     return Path.Combine(GetGameDirectory(GameDirectory.System), "Gothic2.exe");
                case GameFile.SpacerExe:      return Path.Combine(GetGameDirectory(GameDirectory.System), "Spacer.exe");
                case GameFile.Spacer2Exe:     return Path.Combine(GetGameDirectory(GameDirectory.System), "Spacer2.exe");
                case GameFile.GothicIni:      return Path.Combine(GetGameDirectory(GameDirectory.System), "Gothic_GMBT.ini");
                case GameFile.DdrawDll:       return Path.Combine(GetGameDirectory(GameDirectory.System), "ddraw.dll");
                default: throw new FileNotFoundException(file.ToString());
            }
        }

        public string GetGameDirectory(GameDirectory dir)
        {
            switch (dir)
            {
                case GameDirectory.Root:             return Path.GetFullPath(rootDirectory);           
                case GameDirectory.System:           return Path.Combine(GetGameDirectory(GameDirectory.Root), "System");
                case GameDirectory.GD3D11:           return Path.Combine(GetGameDirectory(GameDirectory.System), "GD3D11");
                case GameDirectory.Data:             return Path.Combine(GetGameDirectory(GameDirectory.Root), "Data");
                case GameDirectory.ModVDF:           return Path.Combine(GetGameDirectory(GameDirectory.Data), "ModVDF");
                case GameDirectory.Work:             return Path.Combine(GetGameDirectory(GameDirectory.Root), "_Work");
                case GameDirectory.WorkData:         return Path.Combine(GetGameDirectory(GameDirectory.Work), "Data");
                case GameDirectory.WorkDataOrg:      return Path.Combine(GetGameDirectory(GameDirectory.Work), "DataOriginal");               
                case GameDirectory.ScriptsCompiled:  return Path.Combine(GetGameDirectory(GameDirectory.Scripts), "_compiled");
                case GameDirectory.ScriptsCutscene:  return Path.Combine(GetGameDirectory(GameDirectory.ScriptsContent), "Cutscene");
                case GameDirectory.ScriptsContent:   return Path.Combine(GetGameDirectory(GameDirectory.Scripts), "Content");
                case GameDirectory.AnimsCompiled:    return Path.Combine(GetGameDirectory(GameDirectory.Anims), "_compiled");
                case GameDirectory.MeshesCompiled:   return Path.Combine(GetGameDirectory(GameDirectory.Meshes), "_compiled");
                case GameDirectory.TexturesCompiled: return Path.Combine(GetGameDirectory(GameDirectory.Textures), "_compiled") ;
                default:                             return Path.Combine(GetGameDirectory(GameDirectory.WorkData), Enum.GetName(typeof(GameDirectory), dir));
            }        
        }

        public DirectoryInfo GetGameDirectoryInfo(GameDirectory dir)
        {
            return new DirectoryInfo(GetGameDirectory(dir));
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
