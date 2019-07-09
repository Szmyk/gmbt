using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Szmyk.Utils.Directory;

namespace GMBT
{
    /// <summary> 
    /// Implements merge operations on assets.
    /// </summary>
    internal static class Merge
    {
        [Flags]
        public enum MergeOptions : int
        {
            None = 1,
            All = 2,
            Scripts = 4,
            Worlds = 8,
            Sounds = 16
        }
   
        public static void MergeAssets(Gothic gothic, MergeOptions options)
        {
            string logPattern;

            List<string> mergingPatterns = new List<string>(),
                         mergingOptions  = new List<string>();

            if (options == MergeOptions.All)
            {
                if (Program.Options.InvokedVerb != "pack")
                {
                    new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Scripts)).Delete();
                }                

                mergingPatterns.Add("*");
                logPattern = "Merge.Merging".Translate() + ": {1}";
            }
            else
            {
                logPattern = "Merge.Merging".Translate() + " {0}: {1}";

                if (options.HasFlag(MergeOptions.Scripts))
                {
                    if (Program.Options.InvokedVerb != "pack")
                    {
                        new DirectoryHelper(gothic.GetGameDirectory(Gothic.GameDirectory.Scripts)).Delete();
                    }
                   
                    mergingPatterns.Add("*.d");
                    mergingPatterns.Add("*.src");
                    mergingOptions.Add("Scripts".Translate());                  
                }

                if (options.HasFlag(MergeOptions.Worlds))
                {
                    mergingPatterns.Add("*.zen");
                    mergingOptions.Add("Worlds".Translate());         
                }

                if (options.HasFlag(MergeOptions.Sounds))
                {
                    mergingPatterns.Add("*.wav");
                    mergingOptions.Add("Sounds".Translate());
                }
            }

            foreach (string directoryPath in Program.Config.ModFiles.Assets)
            {            
                DirectoryHelper directory = new DirectoryHelper(directoryPath);

                if (directory.GetNumberOfFiles(mergingPatterns) == 0)
                {
                    continue;
                }

                string message = string.Format(logPattern + "... ", "(" + "Only".Translate() + " " + string.Join(", ", mergingOptions.ToArray()) + ")", directory.Path);
             
                List<string> files = directory.GetFiles(mergingPatterns).ToList();

                if (Program.Config.ModFiles.Exclude != null)
                {
                    foreach (string path in Program.Config.ModFiles.Exclude)
                    {
                        string pattern = path.Split("\\".ToCharArray()).Last();

                        if (string.IsNullOrWhiteSpace(pattern) == false)
                        {
                            FileInfo[] fis = new DirectoryInfo(path.Replace(pattern, string.Empty)).GetFiles(pattern, SearchOption.AllDirectories);

                            foreach (FileInfo fi in fis)
                            {
                                files.RemoveAll(x => Path.GetFullPath(x) == fi.FullName);
                            }
                        }
                    }
                }

                using (ProgressBar mergeBar = new ProgressBar(message, files.Count))
                {               
                    foreach (var file in files)
                    {
                        string directoryPathWithoutRoot = Path.GetFullPath(file).Replace(Path.GetFullPath(directoryPath), string.Empty);

                        string destination = Path.GetFullPath(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)) + directoryPathWithoutRoot;

                        Directory.CreateDirectory(Path.GetDirectoryName(destination));

                        File.Copy(file, destination, true);

                        Logger.Detailed("\t" + directoryPathWithoutRoot);
     
                        mergeBar.Increase();
                    }
                }
            }
        } 
    }
}
