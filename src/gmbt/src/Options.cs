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

        [VerbOption("test",
        HelpText = "Starts a test.")]
        public TestSubOption TestVerb { get; set; }

        [VerbOption("build",
        HelpText = "Starts a VDF build.")]
        public BuildSubOptions BuildVerb { get; set; }

        [VerbOption("update",
        HelpText = "Updates the tool.")]
        public UpdateSubOption UpdateVerb { get; set; }

        public Options()
        {
            Common = new CommonOptions();
            CommonTestBuild = new CommonTestBuildOptions();

            TestVerb = new TestSubOption();
            BuildVerb = new BuildSubOptions();
            UpdateVerb = new UpdateSubOption();
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            var helpText = HelpText.AutoBuild(this, verb).ToString();

            return helpText.Remove(helpText.Length - 1)
                .Replace(HeadingInfo.Default, string.Empty)
                .Replace(CopyrightInfo.Default, string.Empty)
                .Remove(0, 6);    
        }
    }

    /// <summary> 
    /// Represents common arguments that can be used with both "test" and 'build" commands. 
    /// </summary>
    internal class CommonTestBuildOptions : CommonOptions
    {
        [Option('C', "config",
        MetaValue = "<path>",
        DefaultValue = ".gmbt.yml",
        HelpText = "Path to config file. More information in ReadMe.html")]
        public string ConfigFile { get; set; }

        [Option("texturecompile",
        MetaValue = "<normal|quick>",
        DefaultValue = Textures.CompileMode.Normal,
        HelpText = "Method of textures compiling. More information in ReadMe.html.")]
        public Textures.CompileMode TextureCompile { get; set; }

        [Option("noupdatesubtitles",
        HelpText = "Do not update dialogs subtitles.")]
        public bool NoUpdateSubtitles { get; set; }

        [Option("show-compiling-assets",
        HelpText = "Print all compiling by game assets in the console.")]
        public bool ShowCompilingAssets { get; set; }

        [Option("zspy",
        DefaultValue = ZSpy.Mode.None,
        MetaValue = "<none|low|medium|high>",
        HelpText = "Logging level if zSpy.")]
        public ZSpy.Mode ZSpyLevel { get; set; }
    }

    /// <summary> 
    /// Represents common arguments that can be used with all commands. 
    /// </summary>
    internal class CommonOptions
    {
        [Option('L', "lang",
        MetaValue = "<en|pl>",
        HelpText = "Set language of console output.")]
        public string Language { get; set; }

        [Option("help",
        HelpText = "Show this screen.")]

        [ParserState]
        public IParserState LastParserState { get; set; }
    }

    /// <summary> 
    /// Reperesent arguments that can be used with "build" command. 
    /// </summary>
    internal class BuildSubOptions : CommonTestBuildOptions
    {
        [Option('O', "output",
        MetaValue = "<file>",
        MutuallyExclusiveSet = "build",
        HelpText = "Path to VDF output. Default is set in config.")]
        public string Output { get; set; }

        [Option("nopacksounds",
        MutuallyExclusiveSet = "build",
        HelpText = "Do not pack any sounds (WAVs) to VDF.")]
        public bool NoPackSounds { get; set; }

        [Option("comment",
        MutuallyExclusiveSet = "build",
        HelpText = "Set or override comment of VDF. Default is set in config.")]
        public string Comment { get; set; }
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
