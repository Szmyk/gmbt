using System;
using System.IO;
using System.Linq;

using YamlDotNet.Core;

namespace GMBT
{
    static class Program
    {
        public readonly static AppData AppData = new AppData();
        public readonly static Options Options = new Options();   
        public readonly static Updater Updater = new Updater();
        public readonly static HooksManager HooksManager = new HooksManager();

        public static Config Config;       

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(CommandLine.Text.HeadingInfo.Default + Environment.NewLine + CommandLine.Text.CopyrightInfo.Default + Environment.NewLine);

                Internationalization.Init();

                Options.Arguments = args;

                ParseCommandLine(args);

                if (Updater.IsUpdateAvailable)
                {
                    Updater.PrintNotification();
                }
            }
            catch (Exception e)
            {
                Rollbar.Critical(e);

                Logger.Fatal("UnknownError".Translate() + e.ToString());
            }         
        }

        static void ParseCommandLine(string[] args)
        {
            if (CommandLine.Parser.Default.ParseArguments(args, Options,
            (verb, subOptions) =>
            {
                Options.InvokedVerb = verb.ToLower();

                Options.Common = (CommonOptions)subOptions;

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build" || Options.InvokedVerb == "spacer")
                {
                    Options.CommonTestSpacerBuild = (CommonTestSpacerBuildOptions)subOptions;
                }

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build")
                {
                    Options.CommonTestBuild = (CommonTestBuildOptions)subOptions;
                }
            }))
            {
                Logger.Init(Options.Common.Verbosity);
             
                if (Options.Common.Language != null)        
                {
                    Internationalization.SetLanguage(Options.Common.Language);
                }

                if (Options.InvokedVerb == "update")
                {
                    if (Options.UpdateVerb.Force == false)
                    {
                        Logger.Normal("Update.CheckingAvailableUpdate".Translate() + Environment.NewLine);
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

                Options.CommonTestSpacerBuild.ConfigFile = Path.GetFullPath(Options.CommonTestSpacerBuild.ConfigFile);

                if (File.Exists(Options.CommonTestSpacerBuild.ConfigFile) == false)
                {
                    Logger.Fatal("Config.Error.DidNotFound".Translate());
                    return;
                }

                try
                {
                    Config = ConfigDeserializer.Deserialize(Options.CommonTestSpacerBuild.ConfigFile);

                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Options.CommonTestSpacerBuild.ConfigFile));

                    ConfigParser.Parse(Config);

                    if (Config.Predefined != null)
                    {
                        if (args.Length > 1)
                        {
                            var options = Config.Predefined.Where(x => x.ContainsKey(args[1]));

                            if (options.Count() > 0)
                            {
                                var arguments = options.First().First().Value.Split(' ');

                                Logger.Minimal("Options.UsingPredefined".Translate(args[1], string.Join(" ", arguments)));

                                var argsWithoutPredefinedOptionName = args.ToList();

                                argsWithoutPredefinedOptionName.RemoveAt(1);

                                ParseCommandLine(argsWithoutPredefinedOptionName.Concat(arguments).ToArray());

                                return;
                            }
                        }
                    }                

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
               
                using (Gothic gothic = new Gothic(Config.GothicRoot))
                {
                    Logger.InitFileTarget();

                    try
                    {
                        var install = new Install(gothic);

                        install.DetectLastConfigChanges();

                        install.CheckRollbarTelemetry();

                        if (Options.InvokedVerb == "test")
                        {
                            if (install.LastConfigPathChanged()
                            || ( Options.TestVerb.ReInstall ))
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
                            else
                            {
                                if (Directory.Exists(gothic.GetGameDirectory(Gothic.GameDirectory.WorkData)) == false)
                                {
                                    Logger.Fatal("Test.Error.RequireReinstall");
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
                        else if (Options.InvokedVerb == "spacer")
                        {
                            if (install.LastConfigPathChanged())
                            {
                                Logger.Fatal("Install.Error.Reinstall.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                            }
                            else
                            {
                                new Spacer(gothic).Start();
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

                        Rollbar.Critical(e);

                        Logger.Fatal("UnknownError".Translate() + ": {0} {1}\n\n{2}", e.GetType(), e.Message, e.StackTrace);
                    }
                }
            }
        }
    }
}
