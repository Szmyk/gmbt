using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GMBT
{
    using HooksTree = Dictionary<HookMode,
                      Dictionary<HookType,
                      Dictionary<HookEvent, List<string>>>>;

    /// <summary>
    /// Implements deserializing of config file.
    /// </summary>
    internal static class ConfigDeserializer 
    {
        public static Config Deserialize(string configFile)
        {
            DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties();

            Deserializer deserializer = deserializerBuilder.Build();
            StringReader configReader = new StringReader(File.ReadAllText(configFile));

            return deserializer.Deserialize<Config>(configReader);
        }
    }

    /// <summary>
    /// Implements parsing of config file.
    /// </summary>
    internal static class ConfigParser
    {
        public static void Parse (Config config)
        {
            if (config.MinimalVersion != null)
            {
                string localVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;

                if (Updater.IsVersionGreater(localVersion, config.MinimalVersion) == false)
                {
                    Logger.Fatal("MinimalVersionRequired".Translate(config.MinimalVersion));
                }
            }

            if (config.ProjectName == null)
            {
                Logger.Fatal("Config.Error.ProjectNameNotConfigured".Translate());
            }

            if (config.GothicRoot == null)
            {
                Logger.Fatal("Config.Error.RootDirNotConfigured".Translate());
            }

            if (Directory.Exists(config.GothicRoot) == false)
            {
                Logger.Fatal("Config.Error.RootDirDidNotFound".Translate(config.GothicRoot));
            }

            if (config.ModFiles.Assets == null)
            {
                Logger.Fatal("Config.Error.AssetsNotConfigured".Translate());
            }

            if (config.ModFiles.DefaultWorld == null)
            {
                Logger.Fatal("Config.Error.DefaultWorldNotConfigured".Translate());
            }

            if (config.ModVdf.Output == null)
            {
                Logger.Fatal("Config.Error.ModVdfOutputNotConfigured".Translate());
            }

            foreach (var directory in config.ModFiles.Assets)
            {
                if (Directory.Exists(directory) == false)
                {
                    Logger.Fatal("Config.Error.AssetsDirDidNotFound".Translate(directory));
                }
            }

            if (config.Install != null)
            {
                foreach (var dictionary in config.Install)
                {
                    foreach (var entry in dictionary.Keys)
                    {
                        if (File.Exists(entry) == false && Directory.Exists(entry) == false)
                        {
                            Logger.Fatal("Config.Error.PathDidNotFound".Translate(entry));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents the structure of YAML config.
    /// </summary>
    internal class Config
    {
        public string ProjectName { get; set; }

        public string MinimalVersion { get; set; }

        public string GothicRoot { get; set; }

        public ModFiles ModFiles { get; set; }       
        public ModVDF ModVdf { get; set; }

        public List<Dictionary<string, string>> Install { get; set; }
        public List<Dictionary<string, string>> GothicIniOverrides { get; set; }

        public HooksTree Hooks { get; set; }

        public List<Dictionary<string, string>> Predefined { get; set; }
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
