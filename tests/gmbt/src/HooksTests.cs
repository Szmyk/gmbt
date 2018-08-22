using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
