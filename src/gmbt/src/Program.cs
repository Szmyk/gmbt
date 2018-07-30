using System;
using System.IO;

using NLog;

using YamlDotNet.Core;

namespace GMBT
{
    static class Program
    {
        public readonly static AppData AppData = new AppData();
        public readonly static Options Options = new Options();   
        public readonly static Updater Updater = new Updater();
        public readonly static HooksManager HooksManager = new HooksManager();
        public readonly static LogManager LogManager = new LogManager();

        public static Config Config;       
        public static Logger Logger;

        static void Main(string[] args)
        {
            LogManager.InitBasicTargets();
            Logger = LogManager.GetLogger();
          
            Internationalization.Init();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Logger.Fatal("UnknownError".Translate() + e.ExceptionObject.ToString());

            Options.Arguments = args;         

            if (CommandLine.Parser.Default.ParseArguments(args, Options,
            (verb, subOptions) =>
            {
                Options.InvokedVerb = verb.ToLower();

                Options.Common = (CommonOptions)subOptions;

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build")
                {
                    Options.CommonTestBuild = (CommonTestBuildOptions)subOptions;
                }     
            }))
            {               
                if ((Options.Common.Language?.ToLower() == "en")
                ||  (Options.Common.Language?.ToLower() == "pl"))
                {
                    Internationalization.SetLanguage(Options.Common.Language);
                }

                if (Options.InvokedVerb == "update")
                {
                    if (Options.UpdateVerb.Force == false)
                    {
                        Logger.Info("Update.CheckingAvailableUpdate".Translate() + Environment.NewLine);
                    }

                    while (Updater.CheckLatestReleaseTask.Status == System.Threading.Tasks.TaskStatus.Running)
                        ;            

                    if (Updater.FailedCheck)
                    {
                        Logger.Fatal("Update.FailedCheck".Translate());

                        return;
                    }
                    else if (Updater.IsUpdateAvailable || Options.UpdateVerb.Force)
                    {
                        Updater.PrintUpdateInfo();
                    }
                    else
                    {
                        Console.WriteLine("Update.AlreadyUpdated".Translate(), ConsoleColor.Green);
                    }

                    return;
                }

                Options.CommonTestBuild.ConfigFile = Path.GetFullPath(Options.CommonTestBuild.ConfigFile);
            
                if (File.Exists(Options.CommonTestBuild.ConfigFile) == false)
                {
                    Logger.Fatal("Config.Error.DidNotFound".Translate());
                    return;
                }

                try
                {
                    Config = ConfigDeserializer.Deserialize(Options.CommonTestBuild.ConfigFile);

                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Options.CommonTestBuild.ConfigFile));

                    ConfigParser.Parse(Config);

                    if (Config.Hooks != null)
                    {
                        HooksManager.RegisterHooks(Config.Hooks);
                    }
                }
                catch (YamlException e)
                {
                    Logger.Fatal("Config.Error.ParsingError".Translate(e.Message));
                    return;
                }
                catch (Exception e)
                {
                    Logger.Fatal("Config.Error".Translate(e.ToString()));
                    return;
                }

                using (Gothic gothic = new Gothic(Config.GothicRoot))
                {                   
                    LogManager.InitFileTarget();

                    try
                    {
                        var install = new Install(gothic);

                        install.DetectLastConfigChanges();

                        if (Options.InvokedVerb == "test")
                        {
                            if (install.LastConfigPathChanged()
                            || (Options.TestVerb.ReInstall))
                            {
                                if (Options.TestVerb.Full == false)
                                {
                                    if (Options.TestVerb.ReInstall)
                                    {
                                        Logger.Fatal("Install.Error.Reinstall.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                                    }
                                    else
                                    {
                                        Logger.Fatal("Install.Error.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                                    }                                  
                                }
                                else if (Options.TestVerb.Merge != Merge.MergeOptions.All)
                                {
                                    if (Options.TestVerb.ReInstall)
                                    {
                                        Logger.Fatal("Install.Error.Reinstall.RequireMergeAll".Translate() + " " + "Install.Error.RunMergeAll".Translate());
                                    }
                                    else
                                    {
                                        Logger.Fatal("Install.Error.RequireMergeAll".Translate() + " " + "Install.Error.RunMergeAll".Translate());
                                    }
                                }
                                else
                                {
                                    install.Start();
                                }                         
                            }

                            if (Options.TestVerb.Full)
                            {
                                new Test(gothic, TestMode.Full).Start();
                            }
                            else
                            {
                                new Test(gothic, TestMode.Quick).Start();
                            }     
                        }
                        else if (Options.InvokedVerb == "build")
                        {
                            install.Start();
                            new Build(gothic).Start();
                        }
                    }
                    catch (Exception e)
                    {
                        gothic.EndProcess();

                        Logger.Fatal("UnknownError".Translate() + ": {0} {1}\n\n{2}", e.GetType(), e.Message, e.StackTrace);
                    }
                }
            }

            if (Updater.IsUpdateAvailable)
            {
                Updater.PrintNotification();
            }
        }       
    }
}
