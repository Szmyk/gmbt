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
            Rollbar.InitRollbar();

            Console.WriteLine(CommandLine.Text.HeadingInfo.Default + Environment.NewLine + CommandLine.Text.CopyrightInfo.Default + Environment.NewLine);

            Internationalization.Init();
   
            try
            {
                Options.Arguments = args;

                ParseCommandLine(args);

                if (Updater.IsUpdateAvailable)
                {
                    Updater.PrintNotification();
                }
            }
            catch (Exception e)
            {
                bool sentToRollbar = Rollbar.Critical(e);

                Logger.UnknownFatal(e, sentToRollbar);
            }         
        }

        static void ParseCommandLine(string[] args)
        {
            if (CommandLine.Parser.Default.ParseArguments(args, Options,
            (verb, subOptions) =>
            {
                Options.InvokedVerb = verb.ToLower();

                Options.Common = (CommonOptions)subOptions;

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build" || Options.InvokedVerb == "pack" || Options.InvokedVerb == "spacer" || Options.InvokedVerb == "compile")
                {
                    Options.CommonTestSpacerBuildPackCompile = (CommonTestSpacerBuildPackCompileOptions)subOptions;
                }

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build" || Options.InvokedVerb == "spacer" || Options.InvokedVerb == "compile")
                {
                    Options.CommonTestSpacerBuildCompile = (CommonTestSpacerBuildCompileOptions)subOptions;
                }

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "build" || Options.InvokedVerb == "compile")
                {
                    Options.CommonTestBuildCompile = (CommonTestBuildCompileOptions)subOptions;
                }

                if (Options.InvokedVerb == "test" || Options.InvokedVerb == "compile")
                {
                    Options.CommonTestCompile = (CommonTestCompileOptions)subOptions;
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

                Options.CommonTestSpacerBuildPackCompile.ConfigFile = Path.GetFullPath(Options.CommonTestSpacerBuildPackCompile.ConfigFile);

                if (File.Exists(Options.CommonTestSpacerBuildPackCompile.ConfigFile) == false)
                {
                    Logger.Fatal("Config.Error.DidNotFound".Translate());
                    return;
                }

                try
                {
                    Config = ConfigDeserializer.Deserialize(Options.CommonTestSpacerBuildPackCompile.ConfigFile);

                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Options.CommonTestSpacerBuildPackCompile.ConfigFile));

                    ConfigParser.Parse(Config);

                }
                catch (YamlException e)
                {
                    Logger.Fatal("Config.Error.ParsingError".Translate(e.Message));
                    return;
                }              

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

                using (Gothic gothic = new Gothic(Config.GothicRoot))
                {
                    Logger.InitFileTarget();
                   
                    var install = new Install(gothic);

                    install.DetectLastConfigChanges();

                    install.CheckRollbarTelemetry();

                    if (Options.InvokedVerb == "test" || Options.InvokedVerb == "compile")
                    {                      
                        if (install.LastConfigPathChanged()
                        || ( Options.CommonTestCompile.ReInstall ))
                        {
                            if (Options.CommonTestCompile.Full == false)
                            {
                                if (Options.CommonTestCompile.ReInstall)
                                {
                                    Logger.Fatal("Install.Error.Reinstall.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                                }
                                else
                                {
                                    Logger.Fatal("Install.Error.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                                }
                            }
                            else if (Options.CommonTestCompile.Merge != Merge.MergeOptions.All)
                            {
                                if (Options.CommonTestCompile.ReInstall)
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

                        if (Options.InvokedVerb == "test")
                        {
                            if (Options.CommonTestCompile.Full)
                            {
                                if (Options.TestVerb.NoReparse)
                                {
                                    Logger.Fatal("Test.Error.RequireReparse".Translate());
                                }

                                new Test(gothic, TestMode.Full).Start();
                            }
                            else
                            {
                                new Test(gothic, TestMode.Quick).Start();
                            }
                        }
                        else if (Options.InvokedVerb == "compile")
                        {
                            if (Options.CommonTestCompile.Full)
                            {
                                new Compile(gothic, CompileMode.Full).Start();
                            }
                            else
                            {
                                new Compile(gothic, CompileMode.Quick).Start();
                            }
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
                    else if (Options.InvokedVerb == "pack")
                    {
                        if (install.LastConfigPathChanged())
                        {
                            Logger.Fatal("Install.Error.Reinstall.RequireFullTest".Translate() + " " + "Install.Error.RunFullTest".Translate());
                        }
                        else
                        {
                            new Pack(gothic).Start();
                        }                        
                    }
                }
            }
        }
    }
}
