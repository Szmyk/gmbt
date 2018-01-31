using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace GMBT.Tests
{
    [TestClass]
    public class VDFScriptTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            var vdfsScript = new VDFScript(@"C:\ĄĆ\Gothic II Night of the Raven",
                                           @"C:\ĄĆ\Gothic II Night of the Raven\Data\ModVDF\MyMod.mod",
                                           "My amazing mod v1.0",
                                           new List<string>
                                           {
                                                @"_work\data\Meshes\_compiled\*",
                                                @"_work\data\Anims\_compiled\*",
                                                @"_work\data\Sounds\SFX\*",
                                                @"_work\data\Sounds\Speech\*"
                                           }, 
                                           new List<string>
                                           {
                                               
                                           }, 
                                           new List<string>
                                           {
                                                @"_work\Data\Worlds\DK_SUBZENS\*"
                                           });

            var script = vdfsScript.Generate();
        
            string expected =
                @"[BEGINVDF]" + Environment.NewLine +
                @"Comment=My amazing mod v1.0" + Environment.NewLine +
                @"BaseDir=C:\ĄĆ\Gothic II Night of the Raven" + Environment.NewLine +
                @"VDFName=C:\ĄĆ\Gothic II Night of the Raven\Data\ModVDF\MyMod.mod" + Environment.NewLine +
                @"[FILES]" + Environment.NewLine +
                @"_work\data\Meshes\_compiled\*" + Environment.NewLine +
                @"_work\data\Anims\_compiled\*" + Environment.NewLine +
                @"_work\data\Sounds\SFX\*" + Environment.NewLine +
                @"_work\data\Sounds\Speech\*" + Environment.NewLine +
                @"[INCLUDE]" + Environment.NewLine +
                @"[EXCLUDE]" + Environment.NewLine +
                @"_work\Data\Worlds\DK_SUBZENS\*" + Environment.NewLine +
                @"[ENDVDF]";

            Assert.AreEqual(script, expected);
        }
    }
}