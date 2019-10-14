using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GMBT.Tests
{
    [TestClass]
    public class UpdaterTests
    {
        [TestMethod]
        public void VersionTests()
        {
            Assert.AreEqual(new Version(0, 15), Updater.GetVersion("v0.15"));
            Assert.AreEqual(new Version(0, 12), Updater.GetVersion("v0.12-beta"));
            Assert.AreEqual(new Version(1, 0),  Updater.GetVersion("v1.0"));

            Assert.IsTrue(Updater.GetVersion("v0.12")      < Updater.GetVersion("v0.13"));
            Assert.IsTrue(Updater.GetVersion("v0.14")      < Updater.GetVersion("v0.15-beta"));
            Assert.IsTrue(Updater.GetVersion("v0.12")      > Updater.GetVersion("v0.11"));
            Assert.IsTrue(Updater.GetVersion("v0.12-beta") > Updater.GetVersion("v0.11-beta"));
            Assert.IsTrue(Updater.GetVersion("v0.11-beta") < Updater.GetVersion("v0.12-beta"));
        }
    }
}
