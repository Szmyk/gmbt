using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GMBT.Tests
{
    [TestClass]
    public class GothicArgumentsTests
    {
        [TestMethod]
        public void GothicArguments_ToString_Test()
        {
            GothicArguments args = new GothicArguments();

            args.Add("a", "b");
            args.Add("c");

            Assert.IsTrue(args.ToString() == "-a:b -c ");                   
        }
    }
}