using System;
using System.IO;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Conditions;

using CommandLine.Text;

namespace GMBT
{
    /// <summary>
    /// Implements predefined settings for NLog.
    /// </summary>
    internal class LogManager
    {
        private readonly LoggingConfiguration config = new LoggingConfiguration();

        public Logger GetLogger()
        {
            NLog.LogManager.Configuration = config;

            return NLog.LogManager.GetLogger(string.Empty);
        }

        public void InitBasicTargets()
        {
            AddTarget("console", GetConsoleTarget(), LogLevel.Info);
            AddTarget("error", GetErrorTarget(), LogLevel.Fatal);

            Rollbar.InitRollbar();
        }

        public void InitFileTarget()
        {
            AddTarget("file", GetFileTarget(), LogLevel.Trace);

            NLog.LogManager.ReconfigExistingLoggers();
        }

        public void AddTarget(string name, Target target, LogLevel logLevel)
        {
            config.AddTarget(name, target);
            config.LoggingRules.Add(new LoggingRule("*", logLevel, target));       
        }

        public static MethodCallTarget GetErrorTarget()
        {
            return new MethodCallTarget
            {
                ClassName = typeof(LogManager).AssemblyQualifiedName,
                MethodName = "HandleErrorLog"
            };
        }

        public static ColoredConsoleTarget GetConsoleTarget()
        {
            var consoleTarget = new ColoredConsoleTarget
            {
                Layout = @"${message}",
                Header = HeadingInfo.Default + Environment.NewLine + CopyrightInfo.Default + Environment.NewLine,
            };

            consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Info"),
                ForegroundColor = ConsoleOutputColor.Gray
            });

            return consoleTarget;
        }

        public static FileTarget GetFileTarget()
        {
            string fileNameFormat = "${cached:cached=true:inner=${date:format=yyyy-MM-dd/HH-mm-ss}}.log";

            return new FileTarget
            {
                FileName = Path.Combine(Program.AppData.Logs, fileNameFormat),
                Layout = "${longdate} ${message}",
                Header = HeadingInfo.Default + Environment.NewLine + CopyrightInfo.Default + Environment.NewLine + Environment.NewLine +
                "AppData: " + Program.AppData.Path + Environment.NewLine +
                "Working directory: " + Directory.GetCurrentDirectory() + Environment.NewLine +
                "Console arguments: " + String.Join(" ", Program.Options.Arguments) + Environment.NewLine + 
                "Config: " + Environment.NewLine + Environment.NewLine + File.ReadAllText(Program.Options.CommonTestBuild.ConfigFile) + Environment.NewLine,

                DeleteOldFileOnStartup = true
            };
        }

        public static void HandleErrorLog()
        {
            Environment.Exit(-1);
        }
    }
}
