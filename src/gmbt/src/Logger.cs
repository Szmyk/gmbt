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

        public static bool IsFileTargetInit { get => writer != null; }

        public static string LogFilePath;

        private static StreamWriter writer;

        public static void WriteLineToFile(string msg, params object[] arg)
        {
            WriteLineToFile(string.Format(msg, arg));
        }

        public static void WriteLineToFile(string msg)
        {
            if (IsFileTargetInit)
            {
                writer.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss.ffff"), msg);
            }
        }

        public static string ReadStreamToEnd ()
        {
            writer.BaseStream.Position = 0;

            return new StreamReader(writer.BaseStream).ReadToEnd();
        }

        public static void InitFileTarget()
        {
            var now = DateTime.Now;

            var weekBefore = now.AddDays(-7);

            if (Directory.Exists(Program.AppData.Logs) == false)
            {
                Directory.CreateDirectory(Program.AppData.Logs);
            }

            foreach (var dir in Directory.GetDirectories(Program.AppData.Logs))
            {
                var dirName = Path.GetFileName(dir);

                var dateOfLogDirectory = DateTime.FromFileTime(Directory.GetCreationTime(dir).ToFileTime());

                if (dateOfLogDirectory.CompareTo(weekBefore) < 0)
                {
                    Directory.Delete(dir, true);
                }           
            }

            if (Program.Options.CommonTestSpacerBuildPackCompile.CustomLogFile != null)
            {
                LogFilePath = Program.Options.CommonTestSpacerBuildPackCompile.CustomLogFile;
            }
            else
            {
                var fileName = Path.Combine(now.ToString("yyyy-MM-dd"), now.ToString("HH-mm-ss")) + ".log";

                LogFilePath = Path.Combine(Program.AppData.Logs, fileName);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

            writer = new StreamWriter(File.Open(LogFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                AutoFlush = true
                
            };

            writer.Write(
                HeadingInfo.Default + Environment.NewLine + CopyrightInfo.Default + Environment.NewLine + Environment.NewLine +
                "AppData: " + Program.AppData.Path + Environment.NewLine +
                "Working directory: " + Directory.GetCurrentDirectory() + Environment.NewLine +
                "Console arguments: " + String.Join(" ", Program.Options.Arguments) + Environment.NewLine +
                "Config: " + Environment.NewLine + Environment.NewLine + File.ReadAllText(Program.Options.CommonTestSpacerBuildPackCompile.ConfigFile) + Environment.NewLine
            );
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

        public static void Fatal(string msg, bool exitProgram)
        {
            onFatal?.Invoke();

            Console.WriteLine(msg, ConsoleColor.Red);

            WriteLineToFile(msg);

            if (exitProgram)
            {
                Environment.Exit(-1);
            }
        }

        public static void Fatal(string msg)
        {
            Fatal(msg, true);
        }

        public static void UnknownFatal(Exception ex)
        {
            Fatal($"{"UnknownError".Translate()}: {ex.ToString()}", false);

            Environment.Exit(-1);
        }
    }
}
