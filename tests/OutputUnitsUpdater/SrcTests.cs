using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Linq;

namespace OutputUnitsUpdater.Tests
{
    [TestClass]
    public class SrcFileTests
    {
        [TestMethod]
        [DeploymentItem(@"SampleFiles\srcTest", "srcTest")]
        public void GetScriptsTest()
        {
            var scripts = new SrcFile(@"srcTest\scripts.src")
                         .GetScripts()
                         .Select(x => x.ToLower())
                         .ToList();

            Assert.AreEqual(scripts[0], @"srctest\script.d");
            Assert.AreEqual(scripts[1], @"srctest\system\script1.d");
            Assert.AreEqual(scripts[2], @"srctest\system\script2.d");
            Assert.AreEqual(scripts[3], @"srctest\system\script3.d");
            Assert.AreEqual(scripts[4], @"srctest\content\script.d");
            Assert.AreEqual(scripts[5], @"srctest\content\dialogs\npc_1.d");
            Assert.AreEqual(scripts[6], @"srctest\content\dialogs\npc_2.d");
            Assert.AreEqual(scripts[7], @"srctest\content\dialogs\npc_3.d");
            Assert.AreEqual(scripts[8], @"srctest\content\dialogs\npc_4.d");
            Assert.AreEqual(scripts[9], @"srctest\content\dialogs\npc_5.d");
        }
    }
}
