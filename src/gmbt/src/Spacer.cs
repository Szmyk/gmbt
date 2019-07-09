using System.IO;
using System.Diagnostics;
using System;

namespace GMBT
{
    internal class Spacer 
    {
        private Gothic gothic;
        private Process spacerProcess;

        public Spacer (Gothic gothic)
        {
            this.gothic = gothic;

            Logger.SetOnFatalEvent(() =>
            {
                spacerProcess?.Kill();
            });
        }

        public Process RunSpacer ()
        {
            var exeFile = gothic.Version == Gothic.GameVersion.Gothic1
                        ? gothic.GetGameFile(Gothic.GameFile.SpacerExe)
                        : gothic.GetGameFile(Gothic.GameFile.Spacer2Exe);

            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeFile)).Length > 0)
            {
                Logger.Fatal("Spacer.Error.AlreadyRunning");
            }

            ZSpy.Run();

            ProcessStartInfo spacer = new ProcessStartInfo
            {
                FileName = exeFile,
                WindowStyle = ProcessWindowStyle.Maximized,
            };

            spacer.Arguments = GetGothicArguments().ToString();

            Logger.Detailed("Spacer.RunningWithParameters".Translate(spacer.Arguments));

            if (Logger.Verbosity <= VerbosityLevel.Detailed)
            {
                Logger.Minimal("Spacer.Running".Translate());
            }

            spacerProcess = new Process();
            spacerProcess.StartInfo = spacer;

            spacerProcess.Start();

            return spacerProcess;
        }

        public void Start ()
        {
            RunSpacer().WaitForExit();

            ZSpy.Abort();
        }

        public GothicArguments GetGothicArguments()
        {
            GothicArguments parameters = new GothicArguments();

            parameters.Add("ini", Path.GetFileName(gothic.GetGameFile(Gothic.GameFile.GothicIni)));

            parameters.Add("vdfs", "physicalfirst");

            if (Program.Options.CommonTestSpacerBuildCompile.ZSpyLevel != ZSpy.Mode.None)
            {
                parameters.Add("zlog", Convert.ToInt32(Program.Options.CommonTestSpacerBuildCompile.ZSpyLevel) + ",s");
            }

            if (Program.Options.SpacerVerb.MaxFps > 0)
            {
                parameters.Add("zmaxframerate", Program.Options.SpacerVerb.MaxFps);
            }

            if (Program.Options.SpacerVerb.NoAudio)
            {
                if (File.Exists(gothic.GetGameFile(Gothic.GameFile.MusicDat)))
                {
                    parameters.Add("znomusic");
                }

                if (File.Exists(gothic.GetGameFile(Gothic.GameFile.SfxDat)))
                {
                    parameters.Add("znosound");
                }
            }

            return parameters;
        }
    }
}
