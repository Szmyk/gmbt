using System;
using System.IO;

using CommandLine.Text;

namespace GMBT
{
    public enum VerbosityLevel
    {
        Quiet,
        Minimal,
        Normal,
        Detailed,
        Diagnostic
    }

    internal static class Logger
    {
        public static VerbosityLevel Verbosity;

        private static string logFilePath;

        private static StreamWriter writer;

        public static void WriteLineToFile(string msg, params object[] arg)
        {
            WriteLineToFile(string.Format(msg, arg));
        }

        public static void WriteLineToFile(string msg)
        {
            if (fileTargetInited)
            {
                writer.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss.ffff"), msg);
            }
        }

        private static bool fileTargetInited;

        public static void InitFileTarget()
        {
            var now = DateTime.Now;

            var weekBefore = now.AddDays(-7);

            foreach (var dir in Directory.GetDirectories(Program.AppData.Logs))
            {
                var dirName = Path.GetFileName(dir);

                var dateOfLogDirectory = DateTime.FromFileTime(Directory.GetCreationTime(dir).ToFileTime());

                if (dateOfLogDirectory.CompareTo(weekBefore) < 0)
                {
                    Directory.Delete(dir, true);
                }           
            }

            var fileName = Path.Combine(now.ToString("yyyy-MM-dd"), now.ToString("HH-mm-ss")) + ".log";

            logFilePath = Path.Combine(Program.AppData.Logs, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            writer = File.AppendText(logFilePath);

            writer.AutoFlush = true;

            writer.Write(
                HeadingInfo.Default + Environment.NewLine + CopyrightInfo.Default + Environment.NewLine + Environment.NewLine +
                "AppData: " + Program.AppData.Path + Environment.NewLine +
                "Working directory: " + Directory.GetCurrentDirectory() + Environment.NewLine +
                "Console arguments: " + String.Join(" ", Program.Options.Arguments) + Environment.NewLine +
                "Config: " + Environment.NewLine + Environment.NewLine + File.ReadAllText(Program.Options.CommonTestSpacerBuild.ConfigFile) + Environment.NewLine);

            fileTargetInited = true;
        }

        public static void Init(VerbosityLevel level)
        {
            Verbosity = level;
        }

        public static void Diagnostic(string msg)
        {
            if (Verbosity == VerbosityLevel.Diagnostic)
            {
                Console.WriteLine(msg, ConsoleColor.DarkGray);
            }

            WriteLineToFile(msg);
        }

        public static void Detailed(string msg, bool newLine = true)
        {
            if (Verbosity >= VerbosityLevel.Detailed)
            {
                if (newLine)
                {
                    Console.WriteLine(msg, ConsoleColor.DarkGray);
                }
                else
                {
                    Console.Write(msg, ConsoleColor.DarkGray);
                }
            }

            WriteLineToFile(msg);
        }

        public static void Normal(string msg)
        {
            if (Verbosity == VerbosityLevel.Normal)
            {
                Console.WriteLine(msg);
            }

            WriteLineToFile(msg);
        }

        public static void Minimal(string msg)
        {
            if (Verbosity >= VerbosityLevel.Minimal)
            {
                Console.WriteLine(msg);
            }

            WriteLineToFile(msg);
        }

        public static void Warn(string msg)
        {
            if (Verbosity >= VerbosityLevel.Minimal)
            {
                Console.WriteLine(msg, ConsoleColor.Yellow);
            }

            WriteLineToFile(msg);
        }

        public static void Error(string msg)
        {
            if (Verbosity >= VerbosityLevel.Minimal)
            {
                Console.WriteLine(msg, ConsoleColor.Red);
            }

            WriteLineToFile(msg);
        }

        private static Action onFatal;

        public static void SetOnFatalEvent(Action onFatal)
        {
            Logger.onFatal = onFatal;
        }

        public static void Fatal(string msg, params object[] arg)
        {
            Fatal(string.Format(msg, arg));
        }

        public static void Fatal(string msg)
        {
            onFatal?.Invoke();

            Console.WriteLine(msg, ConsoleColor.Red);

            WriteLineToFile(msg);

            Environment.Exit(-1);
        }
    }
}
