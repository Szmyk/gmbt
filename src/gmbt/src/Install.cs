using System;
using System.IO;
using System.Reflection;

using Szmyk.Utils.Paths;
using Szmyk.Utils.Directory;
using Newtonsoft.Json;

namespace GMBT
{
    internal class Install
    {
        private readonly Gothic gothic;

        public static readonly DateTime OriginalAssetsDateTime = new DateTime(1990, 1, 1);
    
        public Install (Gothic gothic)
        {
            this.gothic = gothic;
        }

        public void RenameDisabledVdfs ()
        {
            foreach(var vdf in Directory.EnumerateFiles(gothic.GetGameDirectory(Gothic.GameDirectory.Data), "*", SearchOption.TopDirectoryOnly))
            {
                File.Move(vdf, PathsUtils.GetPathWithoutExtensions(vdf) + ".vdf");
            }
        }

        /// <summary> 
        /// Starts an installation. 
        /// </summary>
        public void Start()
        {
            var configDir = Path.GetFullPath(Path.GetDirectoryName(Program.Options.CommonTestBuild.ConfigFile));

            gothic.GothicINI.Write("last_config_dir", configDir, "GMBT");

            MakeOriginalAssetsBackup();

            RenameDisabledVdfs();

            gothic.GothicINI.Write("gmbtVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "GMBT");
            gothic.GothicINI.Write("testStarts", "0", "GMBT");
            gothic.GothicINI.Write("buildStarts", "0", "GMBT");
        }

        public void CheckRollbarTelemetry ()
        {
            var rollbarSend = gothic.GothicINI.Read("rollbar_send", "GMBT");

            if (rollbarSend == string.Empty)
            {
                Rollbar.Info("New user");

                gothic.GothicINI.Write("rollbar_send", "true", "GMBT");
            }         
        }

        public bool LastConfigPathChanged ()
        {
            var lastConfigDir = gothic.GothicINI.Read("last_config_dir", "GMBT");
            var configDir = Path.GetFullPath(Path.GetDirectoryName(Program.Options.CommonTestBuild.ConfigFile));

            var changed = configDir != lastConfigDir;
            
            if (lastConfigDir == string.Empty)
            {
                Program.Logger.Info("Install.LastConfigPathChanged.FirstRun".Translate());              
            }
            else if (changed && lastConfigDir != string.Empty)
            {
                Program.Logger.Info("Install.LastConfigPathChanged.ConfigChanged".Translate());           
            }

            return changed;
        }

        public void DetectLastConfigChanges ()
        {
            var serializedLastInstallDictionaryFile = gothic.GetGameDirectory(Gothic.GameDirectory.System, true) + "GMBT\\install.json";

            string serializedLastInstallDictionary = null;

            var directory = Path.GetDirectoryName(serializedLastInstallDictionaryFile);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(serializedLastInstallDictionaryFile))
            {
                serializedLastInstallDictionary = File.ReadAllText(serializedLastInstallDictionaryFile);
            }

            var serializedInstallDictionary = JsonConvert.SerializeObject(Program.Config.Install, Formatting.None);

            if (serializedLastInstallDictionary != serializedInstallDictionary)
            {
                if (Program.Config.Install != null)
                {
                    if (Program.Config.Install.Count > 0)
                    {
                        CopyUserFiles();
                    }
                }

                File.WriteAllText(serializedLastInstallDictionaryFile, serializedInstallDictionary);
            }
        }

        /// <summary> 
        /// Copies files defined by user in config ("install" section). 
        /// </summary>
        public void CopyUserFiles()
        {
            var message = "Install.CopyingFiles".Translate();

            Program.Logger.Trace(message);

            using (ProgressBar userFiles = new ProgressBar(message, Program.Config.Install.Count))
            {
                foreach (var dictionary in Program.Config.Install)
                {
                    foreach (var file in dictionary)
                    {
                        File.Copy(file.Key, file.Value, true);

                        Program.Logger.Trace("\t" + file);

                        userFiles.Increase();
                    }
                }
            }
        }

        /// <summary> 
        /// Makes a backup of basic original assets.
        /// </summary>
        public void MakeOriginalAssetsBackup ()
        {
            var message = "Install.PreparingOriginalAssets".Translate();

            Program.Logger.Trace(message);

            using (ProgressBar originalAssets = new ProgressBar(message, 2))
            {
                if (Directory.Exists(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg)))
                {
                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)).Delete();

                    originalAssets.Increase();

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg))
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData));

                    originalAssets.Increase();
                }
                else
                {
                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Video))
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Video");

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Presets))
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Presets");

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Music))
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Music");

                    originalAssets.Increase();

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)).Delete();

                    originalAssets.Increase();

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Video")
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Video));

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Presets")
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Presets));

                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Music")
                                .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Music));
                }
            }               
        }      
    }
}
