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
        public static string LastUuid;

        public static void InitRollbar()
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig("8086308d42f6445bbc44aeab7be68e99")
            {
                #if DEBUG
                    Environment = "development",
                #else
                    Environment = "production",
                #endif

                Person = new Person
                {
                    Id = getUniqueId() ?? Environment.MachineName ?? "unknown",
                    Email = getGitUserEmail() ?? "unknown",
                    UserName = getGitUserName() ?? Environment.UserName ?? "unknown"
                },

                Transform = new Action<Payload>((s) => 
                {               
                    LastUuid = s.Data.Uuid;
                })
            });
        }

        private static Dictionary<string, object> getCustomData()
        {
            return new Dictionary<string, object>()
            {
                { "version", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion },
                { "cmd",  Program.Options.Arguments ?? null },
                { "log",  Logger.IsFileTargetInit ? Logger.ReadStreamToEnd() : null },
                { "config", Program.Config ?? null },
                { "lang", Program.Options.Common.Language ?? null },
                { "lang_cc", Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName ?? null  },
                { "working_dir", Directory.GetCurrentDirectory() ?? null },
                { "appdata", Program.AppData.Path ?? null  },
                { "gothicversion", Program.Config != null ? File.Exists(Path.Combine(Program.Config.GothicRoot, "System\\Gothic2.exe")) ? "gothic2" : "gothic1" : null }
            };
        }

        public static bool Critical(System.Exception ex)
        {
            if (RollbarLocator.RollbarInstance.Config.AccessToken == null)
            {
                InitRollbar();
            }

            try
            {
                RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(5)).Critical(ex, getCustomData());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Info(string message)
        {
            if (RollbarLocator.RollbarInstance.Config.AccessToken == null)
            {
                InitRollbar();
            }

            RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(5)).Info(message, getCustomData());
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
