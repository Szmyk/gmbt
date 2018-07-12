using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;

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

        /// <summary> 
        /// Starts an installation. 
        /// </summary>
        public void Start()
        {
            MakeOriginalAssetsBackup();

            UnPackOriginalVDFs();

            MarkOriginalAssets();

            gothic.GothicINI.Write("gmbtVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "GMBT");
            gothic.GothicINI.Write("testStarts", "0", "GMBT");
            gothic.GothicINI.Write("buildStarts", "0", "GMBT");
        }

        /// <summary>
        /// Runs Gothic VDFS which unpacks original assets. 
        /// </summary>
        public void UnPackOriginalVDFs ()
        {
            var message = "Install.UnpackingOriginalAssets".Translate();

            Program.Logger.Trace(message);

            var files = Directory.GetFiles(gothic.GetGameDirectory(Gothic.GameDirectory.Data), "*", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                File.Move(file, PathsUtils.ChangeExtension(file, ".vdf"));
            }

            using (ProgressBar unpackVDFs = new ProgressBar(message, files.Length))
            {
                foreach (string vdfFile in files)
                {
                    ProcessStartInfo gothicVDFS = new ProcessStartInfo
                    {
                        FileName = Program.AppData.GetTool("GothicVDFS.exe"),
                        Arguments = "/X \"" + PathsUtils.ChangeExtension(vdfFile, ".vdf") + "\"",
                        WorkingDirectory = gothic.GetGameDirectory(Gothic.GameDirectory.Root),
                        WindowStyle = ProcessWindowStyle.Minimized
                    };

                    Process.Start(gothicVDFS).WaitForExit();

                    File.Move(PathsUtils.ChangeExtension(vdfFile, ".vdf"), PathsUtils.ChangeExtension(vdfFile, ".vdf.disabled"));

                    Program.Logger.Trace("\t" + PathsUtils.ChangeExtension(vdfFile, ".vdf"));

                    unpackVDFs.Increase();
                }
            }
              
            if (gothic.Version == Gothic.GameVersion.Gothic1)
            {
                Directory.Delete(gothic.GetGameDirectory(Gothic.GameDirectory.Textures) + "DESKTOP", true);
            }
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

        /// <summary>
        /// Marks original unpacked assets.
        /// </summary>
        public void MarkOriginalAssets ()
        {
            var message = "Install.PreparingOriginalAssets".Translate();

            Program.Logger.Trace(message);

            var files = Directory.GetFiles(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), "*", SearchOption.AllDirectories);

            using (ProgressBar originalAssets = new ProgressBar(message, files.Length))
            {
                foreach (string file in files)
                {
                    File.SetLastWriteTime(file, OriginalAssetsDateTime);
                    File.SetCreationTime(file, OriginalAssetsDateTime);

                    originalAssets.Increase();
                }             
            }
        }        
    }
}
