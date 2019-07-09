using CommandLine;
using CommandLine.Text;

namespace GMBT
{
    /// <summary>
    /// Represents console arguments. 
    /// </summary>
    internal class Options
    {
        public string[] Arguments { get; set; }

        public string InvokedVerb { get; set; }

        public CommonOptions Common { get; set; }
        public CommonTestBuildOptions CommonTestBuild { get; set; }
        public CommonTestSpacerBuildOptions CommonTestSpacerBuild { get; set; }
        public CommonTestSpacerBuildPackOptions CommonTestSpacerBuildPack { get; set; }

        [VerbOption("test",
        HelpText = "Starts a test.")]
        public TestSubOption TestVerb { get; set; }

        [VerbOption("spacer",
        HelpText = "Starts Spacer.")]
        public SpacerSubOption SpacerVerb { get; set; }

        [VerbOption("build",
        HelpText = "Starts a VDF build.")]
        public BuildSubOption BuildVerb { get; set; }

        [VerbOption("pack",
        HelpText = "Packs a VDF volume.")]
        public PackSubOption PackVerb { get; set; }

        [VerbOption("update",
        HelpText = "Updates the tool.")]
        public UpdateSubOption UpdateVerb { get; set; }

        public Options()
        {
            Common = new CommonOptions();
            CommonTestBuild = new CommonTestBuildOptions();
            CommonTestSpacerBuild = new CommonTestSpacerBuildOptions();
            CommonTestSpacerBuildPack = new CommonTestSpacerBuildPackOptions();

            TestVerb = new TestSubOption();
            SpacerVerb = new SpacerSubOption();
            BuildVerb = new BuildSubOption();
            PackVerb = new PackSubOption();
            UpdateVerb = new UpdateSubOption();
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            if (verb?.ToLower() == "help")
            {
                if (Arguments?.Length > 1)
                {
                    verb = Arguments[1];
                }
            }

            var helpText = HelpText.AutoBuild(this, verb).ToString();

            return helpText.Remove(helpText.Length - 1)
                .Replace(HeadingInfo.Default, string.Empty)
                .Replace(CopyrightInfo.Default, string.Empty)
                .Remove(0, 6) + (verb == null ? "\nSee 'gmbt help <command>' or 'gmbt <command> --help' to read about a specific subcommand.\n" : string.Empty);
        }
    }

    /// <summary> 
    /// Represents common arguments that can be used with "test", "spacer", 'build" and "pack" commands. 
    /// </summary>
    internal class CommonTestSpacerBuildPackOptions : CommonOptions
    {
        [Option('C', "config",
        MetaValue = "<path>",
        DefaultValue = ".gmbt.yml",
        HelpText = "Path to config file. More information in ReadMe.html")]
        public string ConfigFile { get; set; }
    }

    /// <summary> 
    /// Represents common arguments that can be used with "test", "spacer" and 'build" commands. 
    /// </summary>
    internal class CommonTestSpacerBuildOptions : CommonTestSpacerBuildPackOptions
    {
        [Option("zspy",
        DefaultValue = ZSpy.Mode.None,
        MetaValue = "<none|low|medium|high>",
        HelpText = "Logging level if zSpy.")]
        public ZSpy.Mode ZSpyLevel { get; set; }
    }

    /// <summary> 
    /// Represents common arguments that can be used with both "test" and 'build" commands. 
    /// </summary>
    internal class CommonTestBuildOptions : CommonTestSpacerBuildOptions
    {
        [Option("noupdatesubtitles",
        HelpText = "Do not update dialogs subtitles.")]
        public bool NoUpdateSubtitles { get; set; }

        [Option("show-duplicated-subtitles",
        HelpText = "Show duplicated subtitles.")]
        public bool ShowDuplicatedSubtitles { get; set; }

        [Option("additional-gothic-parameters",
        HelpText = "Additional Gothic game parameters.")]
        public string AdditionalGothicParameters { get; set; }
    }

    /// <summary> 
    /// Represents common arguments that can be used with all commands. 
    /// </summary>
    internal class CommonOptions
    {
        [Option('L', "lang",
        MetaValue = "<en|pl|sk>",
        HelpText = "Set language of console output.")]
        public string Language { get; set; }

        [Option('V', "verbosity",
        DefaultValue = VerbosityLevel.Normal,
        MetaValue = "<level>",
        HelpText = "Set verbosity level of console output. Levels: quiet|minimal|normal|detailed|diagnostic.")]
        public VerbosityLevel Verbosity { get; set; }

        [Option("help",
        HelpText = "Show this screen.")]

        [ParserState]
        public IParserState LastParserState { get; set; }
    }

    /// <summary> 
    /// Reperesent arguments that can be used with "build" command. 
    /// </summary>
    internal class BuildSubOption : CommonTestBuildOptions
    {
        [Option('O', "output",
        MetaValue = "<file>",
        HelpText = "Path to VDF output. Default is set in config.")]
        public string Output { get; set; }

        [Option("nopacksounds",
        HelpText = "Do not pack any sounds (WAVs) to VDF.")]
        public bool NoPackSounds { get; set; }

        [Option("comment",
        HelpText = "Set or override comment of VDF. Default is set in config.")]
        public string Comment { get; set; }
    }

    /// <summary> 
    /// Reperesent arguments that can be used with "pack" command. 
    /// </summary>
    internal class PackSubOption : CommonTestSpacerBuildPackOptions
    {
        [Option('O', "output",
        MetaValue = "<file>",
        HelpText = "Path to VDF output. Default is set in config.")]
        public string Output { get; set; }

        [Option("nopacksounds",
        HelpText = "Do not pack any sounds (WAVs) to VDF.")]
        public bool NoPackSounds { get; set; }

        [Option("comment",
        HelpText = "Set or override comment of VDF. Default is set in config.")]
        public string Comment { get; set; }
    }

    /// <summary> 
    /// Reperesent arguments that can be used with "spacer" command. 
    /// </summary>
    internal class SpacerSubOption : CommonTestSpacerBuildOptions
    {
        [Option("noaudio",
        HelpText = "Run Spacer with no sounds and music.")]
        public bool NoAudio { get; set; }

        [Option("maxfps",
        HelpText = "Maximum framerate.")]
        public int MaxFps { get; set; }
    }
    
    /// <summary> 
    /// Reperesent arguments that can be used with "test" command. 
    /// </summary>
    internal class TestSubOption : CommonTestBuildOptions
    {
        [Option('F', "full",
        HelpText = "Start full test.")]
        public bool Full { get; set; }

        [Option('W', "world",
        MetaValue = "<zen>",
        HelpText = "Name of world to start a game.")]
        public string World { get; set; }

        [Option("merge",
        MetaValue = "<none|all|scripts|worlds|sounds>",
        DefaultValue = GMBT.Merge.MergeOptions.All,
        HelpText = "Choose what do you want to merge.")]
        public Merge.MergeOptions Merge { get; set; }

        [Option("windowed",
        HelpText = "Run Gothic game in window.")]
        public bool RunGothicWindowed { get; set; }

        [Option("noaudio",
        HelpText = "Run Gothic with no sounds and music.")]
        public bool NoAudio { get; set; }

        [Option("ingametime",
        MetaValue = "<hour:minute>",
        HelpText = "Ingame time.")]
        public string InGameTime { get; set; }

        [Option("nodx11",
        HelpText = "If D3D11-Renderer for Gothic is installed, turn off this wrapper.")]
        public bool NoDirectX11 { get; set; }

        [Option("nomenu",
        HelpText = "Run game without menu showing (start new game immediately).")]
        public bool NoMenu { get; set; }

        [Option('R', "reinstall",
        HelpText = "Reinstall before test.")]
        public bool ReInstall { get; set; }

        [Option('D', "devmode",
        HelpText = "Dev mode of game (marvin mode).")]
        public bool DevMode { get; set; }
    }

    /// <summary> 
    /// Reperesent arguments that can be used with "update" command. 
    /// </summary>
    internal class UpdateSubOption : CommonOptions
    {
        [Option('F', "force",
        HelpText = "Download and update even if it is up to date.")]
        public bool Force { get; set; }

        [Option("no-confirm",
        HelpText = "Do not ask for download.")]
        public bool NoConfirm { get; set; }
    }
}
