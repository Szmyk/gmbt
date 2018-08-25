using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GMBT.Tests
{
    [TestClass]
    public class UpdaterTests
    {
        [TestMethod]
        public void IsVersionGreaterTest()
        {
            Assert.AreEqual(false, Updater.IsVersionGreater("v0.15", "v0.15"));
            Assert.AreEqual(false, Updater.IsVersionGreater("v0.12", "v0.13"));
            Assert.AreEqual(true,  Updater.IsVersionGreater("v0.13", "v0.12"));
            Assert.AreEqual(true,  Updater.IsVersionGreater("v0.12", "v0.12-beta"));
            Assert.AreEqual(false, Updater.IsVersionGreater("v0.14", "v0.15-beta"));
            Assert.AreEqual(true,  Updater.IsVersionGreater("v0.12", "v0.11"));
            Assert.AreEqual(true,  Updater.IsVersionGreater("v0.12-beta", "v0.11-beta"));
            Assert.AreEqual(false, Updater.IsVersionGreater("v0.11-beta", "v0.12-beta"));
        }
    }
}
