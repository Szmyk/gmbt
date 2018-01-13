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
    internal static class LogManager
    {
        /// <summary> 
        /// Returns predefined NLog logger settings.
        /// </summary>
        public static Logger GetLogger()
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

            var config = new LoggingConfiguration();

            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));

            string fileNameFormat = "${cached:cached=true:inner=${date:format=yyyy-MM-dd/HH-mm-ss}}.log";

            var fileTarget = new FileTarget
            {
                FileName = Program.AppData.Logs + fileNameFormat,
                Layout = "${longdate} ${message}",
                Header = HeadingInfo.Default + Environment.NewLine + CopyrightInfo.Default + Environment.NewLine + Environment.NewLine + "AppData: " + Program.AppData.Path + Environment.NewLine + "Working directory: " + Directory.GetCurrentDirectory() + Environment.NewLine,

                DeleteOldFileOnStartup = true
            };
            
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));

            var errorTarget = new MethodCallTarget
            {
                ClassName = typeof(LogManager).AssemblyQualifiedName,
                MethodName = "HandleErrorLog"
            };

            config.AddTarget("error", errorTarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Fatal, errorTarget));

            NLog.LogManager.Configuration = config;

            return NLog.LogManager.GetLogger(string.Empty);
        }

        public static void HandleErrorLog()
        {
            Environment.Exit(-1);
        }
    }
}
