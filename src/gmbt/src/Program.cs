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
        public readonly static Logger Logger = LogManager.GetLogger();
        public readonly static Updater Updater = new Updater();

        public static Config Config;

        static void Main(string[] args)
        {
            Internationalization.Init();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Logger.Fatal("UnknownError".Translate() + e.ExceptionObject.ToString());

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
                        Console.Write("Update.CheckingAvailableUpdate".Translate() + Environment.NewLine + Environment.NewLine);
                    }

                    while (Updater.CheckLatestReleaseTask.Status == System.Threading.Tasks.TaskStatus.Running)
                        ;            

                    if (Updater.FailedCheck)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Update.FailedCheck".Translate());
                        Console.ForegroundColor = ConsoleColor.Gray;

                        return;
                    }
                    else if (Updater.IsUpdateAvailable || Options.UpdateVerb.Force)
                    {
                        Updater.PrintUpdateInfo();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Update.AlreadyUpdated".Translate());
                        Console.ForegroundColor = ConsoleColor.Gray;
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

                    Logger.Trace("Config (" + Options.CommonTestBuild.ConfigFile + "):\n" + File.ReadAllText(Options.CommonTestBuild.ConfigFile));
                }
                catch (YamlException e)
                {
                    Logger.Fatal("Config.Error.ParsingError".Translate(e.Message));
                    return;
                }
                catch (Exception e)
                {
                    Logger.Fatal("Config.Error".Translate(e.Message));
                    return;
                }

                using (Gothic gothic = new Gothic(Config.GothicRoot))
                {                 
                    Logger.Trace("Console arguments: " + String.Join(" ", args));

                    System.Windows.Forms.Application.ApplicationExit += new EventHandler((x, y) => gothic.EndProcess());

                    try
                    {
                        if (Options.InvokedVerb == "test")
                        {
                            if ((string.Equals(gothic.GetLastUsedConfigPath(), Options.CommonTestBuild.ConfigFile, StringComparison.CurrentCultureIgnoreCase) == false)
                            || (Options.TestVerb.ReInstall))
                            {
                                new Install(gothic).Start();
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
                            new Install(gothic).Start();
                            new Build(gothic).Start();
                        }
                    }
                    catch (Exception e)
                    {
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
