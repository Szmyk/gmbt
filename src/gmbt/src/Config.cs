using System.IO;
using System.Collections.Generic;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using I18NPortable;

namespace GMBT
{
    /// <summary>
    /// Implements deserializing of config file.
    /// </summary>
    internal static class ConfigDeserializer 
    {
        public static Config Deserialize(string configFile)
        {
            DeserializerBuilder deserializerBuilder = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention());
            Deserializer deserializer = deserializerBuilder.Build();
            StringReader configReader = new StringReader(File.ReadAllText(configFile));

            Config config = deserializer.Deserialize<Config>(configReader);
            
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Program.Options.CommonTestBuild.ConfigFile));

            if (Directory.Exists(config.GothicRoot) == false)
            {
                throw new DirectoryNotFoundException(string.Format("Config.Error.RootDirDidNotFound".Translate(), Path.GetFullPath(config.GothicRoot)));
            }

            foreach (var directory in config.ModFiles.Assets)
            {
                if (Directory.Exists(directory) == false)
                {
                    throw new DirectoryNotFoundException(string.Format("Config.Error.AssetsDirDidNotFound".Translate(), Path.GetFullPath(directory)));
                }
            }

            if (config.Install != null)
            {
                foreach (var dictionary in config.Install)
                {
                    foreach (var file in dictionary.Keys)
                    {
                        if (File.Exists(file) == false)
                        {
                            throw new FileNotFoundException(string.Format("Config.Error.FileDidNotFound".Translate(), Path.GetFullPath(file)));
                        }
                    }
                }
            }

            return config;
        }
    }

    /// <summary>
    /// Represents the structure of YAML config.
    /// </summary>
    internal class Config
    {    
        public string GothicRoot { get; set; }

        public ModFiles ModFiles { get; set; }       
        public ModVDF ModVdf { get; set; }

        public List<Dictionary<string, string>> Install { get; set; }
        public List<Dictionary<string, string>> GothicIniOverrides { get; set; }  
    }

    internal class ModFiles
    {
        public List<string> Assets { get; set; }
        public List<string> Exclude { get; set; }

        public string DefaultWorld { get; set; }
    }

    internal class ModVDF
    {
        public string Output { get; set; }
        public string Comment { get; set; }

        public List<string> Include { get; set; } 
        public List<string> Exclude { get; set; }
    }
}
