using System;
using System.IO;

using NLog;

using YamlDotNet.Core;

using I18NPortable;

namespace GMBT
{
    static class Program
    {
        public readonly static AppData AppData = new AppData();
        public readonly static Options Options = new Options();
        public readonly static Logger Logger = LogManager.GetLogger();

        public static Config Config;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Logger.Fatal("UnknownError".Translate() + e.ExceptionObject.ToString());

            I18N.Current.
                SetFallbackLocale("en").
                Init(System.Reflection.Assembly.GetExecutingAssembly());

            if (CommandLine.Parser.Default.ParseArguments(args, Options,
            (verb, subOptions) =>
            {
                Options.InvokedVerb = verb;
                Options.Common = (CommonOptions)subOptions;
            }))
            {
                if ((Options.Common.Language == "en")
                ||  (Options.Common.Language == "pl"))
                {
                    I18N.Current.Locale = Options.Common.Language;
                }

                Options.Common.ConfigFile = Path.GetFullPath(Options.Common.ConfigFile);

                if (File.Exists(Options.Common.ConfigFile) == false)
                {
                    Logger.Fatal("Config.Error.DidNotFound".Translate());
                    return;
                }

                try
                {
                    Config = ConfigDeserializer.Deserialize(Options.Common.ConfigFile);

                    Logger.Trace("Config (" + Options.Common.ConfigFile + "):\n" + File.ReadAllText(Options.Common.ConfigFile));
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
                            if ((string.Equals(gothic.GetLastUsedConfigPath(), Options.Common.ConfigFile, StringComparison.CurrentCultureIgnoreCase) == false)
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
        }       
    }
}
