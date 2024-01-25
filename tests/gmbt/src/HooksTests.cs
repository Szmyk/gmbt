using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GMBT.Tests
{
    [TestClass]
    public class HooksTests
    {
        [TestMethod]
        [DeploymentItem("SampleFiles\\hooks.yml")]
        public void Hooks_Test()
        {
            var config = ConfigDeserializer.Deserialize("hooks.yml");

            var manager = new HooksManager();

            manager.RegisterHooks(config.Hooks);

            Assert.AreEqual(manager.Hooks[0], new Hook {
                Mode = HookMode.Common,
                Type = HookType.Post,
                Event = HookEvent.AssetsMerge,
                Command = "a"
            });

            Assert.AreEqual(manager.Hooks[1], new Hook
            {
                Mode = HookMode.Common,
                Type = HookType.Post,
                Event = HookEvent.AssetsMerge,
                Command = "b"
            });

            Assert.AreEqual(manager.Hooks[2], new Hook
            {
                Mode = HookMode.Test,
                Type = HookType.Post,
                Event = HookEvent.SubtitlesUpdate,
                Command = "d"
            });
        }

        [TestMethod]
        [DeploymentItem("SampleFiles\\exit_error.bat")]
        [DeploymentItem("SampleFiles\\hooks.fail.yml")]
        public void Hooks_FatalOnHookError()
        {
            Internationalization.Init("en");
            Logger.Verbosity = VerbosityLevel.Minimal;

            var config = ConfigDeserializer.Deserialize("hooks.fail.yml");

            var manager = new HooksManager();

            manager.RegisterHooks(config.Hooks);

            var stringWriter = new StringWriter();
            System.Console.SetOut(stringWriter);

            manager.RunHooks(HookMode.Test, HookType.Post, HookEvent.AssetsMerge);

            var output = stringWriter.ToString();

            Assert.IsTrue(output.Contains("Hook exited with error code: -1"));
        }


        [TestMethod]
        [DeploymentItem("SampleFiles\\stdout.bat")]
        [DeploymentItem("SampleFiles\\hooks.stdout.yml")]
        public void Hooks_StandardOutput()
        {
            Internationalization.Init("en");
            Logger.Verbosity = VerbosityLevel.Detailed;

            var config = ConfigDeserializer.Deserialize("hooks.stdout.yml");

            var manager = new HooksManager();

            manager.RegisterHooks(config.Hooks);

            var stringWriter = new StringWriter();
            System.Console.SetOut(stringWriter);

            manager.RunHooks(HookMode.Test, HookType.Post, HookEvent.AssetsMerge);

            var output = stringWriter.ToString();

            Assert.IsTrue(output.Contains("Message to stdout #1"));
            Assert.IsTrue(output.Contains("Message to stdout #2"));
            Assert.IsTrue(output.Contains("Message to stderr #1"));
        }

        [TestMethod]
        [DeploymentItem("SampleFiles\\parameters.bat")]
        [DeploymentItem("SampleFiles\\hooks.parameters.yml")]
        public void Hooks_ForwardParameters()
        {
            Internationalization.Init("en");
            Logger.Verbosity = VerbosityLevel.Detailed;

            var config = ConfigDeserializer.Deserialize("hooks.parameters.yml");

            var manager = new HooksManager();

            manager.RegisterHooks(config.Hooks);

            var stringWriter = new StringWriter();
            System.Console.SetOut(stringWriter);

            Program.Options.CommonTestSpacerBuildPackCompile.HooksForwardParameter = "ForwardParameter";

            manager.RunHooks(HookMode.Test, HookType.Post, HookEvent.AssetsMerge);

            var output = stringWriter.ToString();

            Assert.IsTrue(output.Contains("Parameter detected"));
        }

        [TestMethod]
        [DeploymentItem("SampleFiles\\parameters.bat")]
        [DeploymentItem("SampleFiles\\hooks.parameters.yml")]
        public void Hooks_NoForwardParameters()
        {
            Internationalization.Init("en");
            Logger.Verbosity = VerbosityLevel.Detailed;

            var config = ConfigDeserializer.Deserialize("hooks.parameters.yml");

            var manager = new HooksManager();

            manager.RegisterHooks(config.Hooks);

            var stringWriter = new StringWriter();
            System.Console.SetOut(stringWriter);

            Program.Options.CommonTestSpacerBuildPackCompile.HooksForwardParameter = null;

            manager.RunHooks(HookMode.Test, HookType.Post, HookEvent.AssetsMerge);

            var output = stringWriter.ToString();

            Assert.IsTrue(output.Contains("No parameters"));
        }
    }
}
