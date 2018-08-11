using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.NetworkInformation;

using Rollbar;
using Rollbar.DTOs;

namespace GMBT
{
    internal static class Rollbar
    {
        public static void InitRollbar()
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig("a6658daddb494506b591ea8cd41370ac")
            {
                Environment = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion,
                Person = new Person
                {
                    Id = getUniqueId() ?? Environment.MachineName ?? "unknown",
                    Email = getGitUserEmail() ?? "unknown",
                    UserName = getGitUserName() ?? Environment.UserName ?? "unknown"
                }
            });
        }

        private static Dictionary<string, object> getCustomData()
        {
            return new Dictionary<string, object>()
            {
                { "cmd", string.Join(" ", Program.Options.Arguments) },
                { "config", Program.Config },
                { "lang", Program.Options.Common.Language },
                { "lang_cc", Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName },
                { "working_dir", Directory.GetCurrentDirectory() },
                { "appdata", Program.AppData.Path },
                { "gothicversion", File.Exists(Path.Combine(Program.Config.GothicRoot, "System\\Gothic2.exe")) ? "gothic2" : "gothic1" }
            };
        }

        public static void Critical(System.Exception ex)
        {
            RollbarLocator.RollbarInstance.Critical(ex, getCustomData());
        }

        public static void Info(string message)
        {
            RollbarLocator.RollbarInstance.Info(message, getCustomData());
        }

        private static string getGitCmdOutput(string arguments)
        {
            try
            {
                Process git = new Process();

                git.StartInfo.FileName = "git.exe";
                git.StartInfo.Arguments = arguments;
                git.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                git.StartInfo.RedirectStandardOutput = true;
                git.StartInfo.CreateNoWindow = true;
                git.StartInfo.UseShellExecute = false;

                git.Start();
                git.WaitForExit();

                return git.StandardOutput.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        private static string getGitUserEmail()
        {
            return getGitCmdOutput("config --global user.email");
        }

        private static string getGitUserName()
        {
            return getGitCmdOutput("config --global user.name");
        }

        private static string getUniqueId()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
