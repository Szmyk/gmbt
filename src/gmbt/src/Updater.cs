using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

using Szmyk.Utils.BytesHelper;

using I18NPortable;

using Newtonsoft.Json;

namespace GMBT
{
    class Release
    {
        public string Version { get; set; }
        public string ArtifactName { get; set; }
        public Uri ArtifactDownloadUrl { get; set; }
        public string Notes { get; set; }
        public long Size { get; set; }
    }

    class Updater
    {
        public bool FailedCheck { get; set; }

        public bool IsUpdateAvailable { get; set; }
        public Release LatestRelease { get; set; }
        public Task CheckLatestReleaseTask { get; set; }

        public Updater ()
        {
            CheckLatestReleaseTask = Task.Run(() =>
            {
                try
                {
                    LatestRelease = GetLatestRelease();

                    string localVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;

                    IsUpdateAvailable = LatestRelease.Version != localVersion;
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
            Console.WriteLine();
        }

        public void PrintUpdateInfo()
        {
            if (Program.Options.UpdateVerb.Force == false)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Update.NewUpdateAvailable".Translate() + ":" + Environment.NewLine);

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

                Console.Write("Update.DoYouWantToUpdate".Translate(BytesHelper.ToString(LatestRelease.Size)) + " [y/n] ");

                key = Console.ReadKey().Key;

                Console.WriteLine();
               
                if (key == ConsoleKey.Y)
                {
                    Console.WriteLine();

                    try
                    {
                       DownloadLastRelease();
                    }
                    catch
                    {

                    }
                }
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);
        }

        public void PrintNotification()
        {
            Console.Write(Environment.NewLine + "Update.Notification".Translate(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion + " -> " + LatestRelease.Version) + " ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("gmbt update");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(".");
        }

        public void DownloadLastRelease()
        {
            Directory.CreateDirectory(Program.AppData.Path + "updates\\" + LatestRelease.Version);
            string localPath = Program.AppData.Path + "updates\\" + LatestRelease.Version + "\\" + LatestRelease.ArtifactName;

            Console.Write("Update.Downloading".Translate() + " ");

            using (ProgressBar downloadBar = new ProgressBar(100))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (o, e) => downloadBar.SetProgress(e.ProgressPercentage);

                    client.DownloadFileAsync(LatestRelease.ArtifactDownloadUrl, localPath);

                    while (client.IsBusy)
                        ;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Environment.NewLine + "Update.Ready".Translate());
            Console.ForegroundColor = ConsoleColor.Gray;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler((x, y) =>
            {
                Process.Start(localPath, string.Format(@"/S /D //""{0}""//", Program.AppData.Path));
            });

            Environment.Exit(0);
        }
    }
}
