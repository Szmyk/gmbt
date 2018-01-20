using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Szmyk.Utils.Paths;
using Szmyk.Utils.Directory;

using I18NPortable;

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
            gothic.GothicINI.Write("lastConfigPath", string.Empty, "GMBT");

            MakeOriginalAssetsBackup();

            UnPackOriginalVDFs();

            if (Program.Config.Install != null)
            {
                if (Program.Config.Install.Count > 0)
                {
                    CopyUserFiles();
                }
            }

            MarkOriginalAssets();

            gothic.GothicINI.Write("lastConfigPath", Program.Options.CommonTestBuild.ConfigFile, "GMBT");
            gothic.GothicINI.Write("gmbtVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "GMBT");
            gothic.GothicINI.Write("testStarts", "0", "GMBT");
            gothic.GothicINI.Write("buildStarts", "0", "GMBT");
        }

        /// <summary>
        /// Runs Gothic VDFS which unpacks original assets. 
        /// </summary>
        public void UnPackOriginalVDFs ()
        {
            Console.Write("Install.UnpackingOriginalAssets".Translate() + " ");
            Program.Logger.Trace("Install.UnpackingOriginalAssets".Translate());

            int filesCount = 0;

            foreach (string file in Directory.GetFiles(gothic.GetGameDirectory(Gothic.GameDirectory.Data), "*", SearchOption.TopDirectoryOnly))
            {
                File.Move(file, PathsUtils.ChangeExtension(file, ".vdf"));
                filesCount = filesCount + 1;
            }

            string gothicVdfsPath = Program.AppData.Tools + "GothicVDFS.exe";

            using (ProgressBar unpackVDFs = new ProgressBar(filesCount))
            {
                foreach (string vdfFile in Directory.EnumerateFiles(gothic.GetGameDirectory(Gothic.GameDirectory.Data), "*.vdf", SearchOption.TopDirectoryOnly))
                {
                    ProcessStartInfo gothicVDFS = new ProcessStartInfo()
                    {
                        FileName = gothicVdfsPath,
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

        /// <summary> 
        /// Copies files defined by user in config ("install" section). 
        /// </summary>
        public void CopyUserFiles()
        {
            Console.Write("Install.CopyingFiles".Translate() + " ");
            Program.Logger.Trace("Install.CopyingFiles".Translate());

            using (ProgressBar userFiles = new ProgressBar(Program.Config.Install.Count))
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
            Console.Write("Install.PreparingOriginalAssets".Translate() + " ");
            Program.Logger.Trace("Install.PreparingOriginalAssets".Translate());

            if (Directory.Exists(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg)))
            {
                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)).Delete();

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg))
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData));
            }
            else
            {
                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Video))
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Video");

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Presets))
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Presets");

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Music))
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Music");

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)).Delete();

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Video")
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Video));

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Presets")
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Presets));

                new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.WorkDataOrg) + "Music")
                            .CopyTo(gothic.GetGameDirectory(Gothic.GameDirectory.Music));
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Marks original unpacked assets.
        /// </summary>
        public void MarkOriginalAssets ()
        {
            Console.Write("Install.PreparingOriginalAssets".Translate() + " ");
            Program.Logger.Trace("Install.PreparingOriginalAssets".Translate() );

            foreach (string file in Directory.GetFiles(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData), "*", SearchOption.AllDirectories))
            {
                File.SetLastWriteTime(file, OriginalAssetsDateTime);
                File.SetCreationTime(file, OriginalAssetsDateTime);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;
        }        
    }
}
