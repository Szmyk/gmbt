using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

using Szmyk.Utils.BytesHelper;

using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace GMBT
{
    public class Release
    {
        public string Version { get; set; }
        public string ArtifactName { get; set; }
        public Uri ArtifactDownloadUrl { get; set; }
        public string Notes { get; set; }
        public long Size { get; set; }
    }

    public class Updater
    {
        public bool FailedCheck { get; set; }

        public bool IsUpdateAvailable { get; set; }
        public Release LatestRelease { get; set; }
        public Task CheckLatestReleaseTask { get; set; }

        public static bool IsVersionGreater(string v1, string v2)
        {
            var rx = new Regex(@"v([\d.]+)([-\w]+)*");

            var v1Splitted = rx.Split(v1);
            var v2Splitted = rx.Split(v2);

            var version1 = v1Splitted[1];
            var version2 = v2Splitted[1];

            Version versionA = new Version(version1);
            Version versionB = new Version(version2);

            if (v2Splitted.Length == 2)
            {
                return false;
            }
            else
            {
                return versionA.CompareTo(versionB) >= 0;
            }
        }

        public Updater ()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CheckLatestReleaseTask = Task.Run(() =>
            {
                try
                {
                    LatestRelease = GetLatestRelease();

                    string localVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;

                    IsUpdateAvailable = IsVersionGreater(LatestRelease.Version, localVersion);
                }
                catch              
                {
                    FailedCheck = true;
                }              
            });
        }

        public Release GetLatestRelease()
        {
            var uri = new Uri("https://api.github.com/repos/szmyk/gmbt/releases/latest");
            var request = WebRequest.CreateHttp(uri);
            request.UserAgent = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductName;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                FailedCheck = true;
            }            

            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                
            dynamic json = JsonConvert.DeserializeObject(reader.ReadToEnd());
            
            return new Release
            {
                Version = json.tag_name.ToString(),
                Notes = json.body.ToString(),
                ArtifactName = json.assets[0].name.ToString(),
                ArtifactDownloadUrl = new Uri(json.assets[0].browser_download_url.ToString()),
                Size = json.assets[0].size
            };                   
        }

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        private void renderUpdateNotes(string body)
        {
            Console.WriteLine();

            foreach (var s in body.Split('\n'))
            {
                Console.WriteLine("\t" + s.Replace("# ", string.Empty).Replace("#", string.Empty));
            }

            Console.WriteLine();
        }

        public void PrintUpdateInfo()
        {
            if (Program.Options.UpdateVerb.Force == false)
            {
                Console.WriteLine("Update.NewUpdateAvailable".Translate() + ":" + Environment.NewLine);

                Console.WriteLine("\t" + "Update.Version".Translate() + ": " + LatestRelease.Version);

                renderUpdateNotes(LatestRelease.Notes);
            }      

            if (Program.Options.UpdateVerb.NoConfirm || Program.Options.UpdateVerb.Force)
            {
                DownloadLastRelease();

                return;
            }

            ConsoleKey key;

            do
            {
                ClearLastLine();

                Console.WriteLine();
                Console.Write("Update.DoYouWantToUpdate".Translate(BytesHelper.ToString(LatestRelease.Size)) + " [y/n] ");

                key = Console.ReadKey().Key;

                Console.WriteLine();
               
                if (key == ConsoleKey.Y)
                {
                    Console.WriteLine();
           
                    DownloadLastRelease();                
                }
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);
        }

        public void PrintNotification()
        {
            var localVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;

            Console.WriteLine();
            Console.WriteSecondInColor("Update.Notification".Translate(localVersion + " -> " + LatestRelease.Version) + " ", 
                                       "gmbt update",
                                       ".", 
                                       ConsoleColor.Cyan);
        }

        public void DownloadLastRelease()
        {
            string localPath = Path.GetTempFileName() + ".exe.";

            ProgressBar downloadBar = new ProgressBar("Update.Downloading".Translate(), 100);

            WebClient client = new WebClient();

            Exception error = null;

            client.DownloadFileCompleted += ((sender, args) =>
            {
                error = args.Error;              
            });

            client.DownloadProgressChanged += (o, e) => downloadBar.SetProgress(e.ProgressPercentage);

            client.DownloadFileAsync(LatestRelease.ArtifactDownloadUrl, localPath);

            while (client.IsBusy)
                ;

            if (error == null)
            {
                downloadBar.Dispose();

                Console.WriteLine("Update.Ready".Translate(), ConsoleColor.Green);

                AppDomain.CurrentDomain.ProcessExit += new EventHandler((x, y) =>
                {
                    Process.Start(localPath, string.Format(@"/S /D //""{0}""//", Program.AppData.Path));
                });

                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine(Environment.NewLine + "Update.FailedDownload".Translate(), ConsoleColor.Red);
                Environment.Exit(-1);
            }
        }    
    }
}
