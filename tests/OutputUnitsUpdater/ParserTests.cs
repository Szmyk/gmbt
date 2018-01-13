using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace OutputUnitsUpdater.Tests
{
    [TestClass]
    public class OutputUnitsParserTests
    {
        [TestMethod]
        [DeploymentItem("SampleFiles\\Dialog.d")]
        public void OutputUnitsParser_ParseDialog_Test()
        {
            OutputUnitsParser outputUnitsParser = new OutputUnitsParser("Dialog.d", OutputUnitsParser.ScriptType.NormalDialogue);

            List<OutputUnitInfo> ouInfos = outputUnitsParser.Parse();

            Assert.AreEqual(ouInfos[0].Name, "DIA_Npc_Test_15_00");
            Assert.AreEqual(ouInfos[0].Text, ".");

            Assert.AreEqual(ouInfos[1].Name, "DIA_Npc_Test_01_01");
            Assert.AreEqual(ouInfos[1].Text, "..");

            Assert.AreEqual(ouInfos[2].Name, "DIA_Npc_Test_01_02");
            Assert.AreEqual(ouInfos[2].Text, "...");

            Assert.AreEqual(ouInfos[3].Name, "DIA_Npc_Test_15_03");
            Assert.AreEqual(ouInfos[3].Text, ".");

            Assert.AreEqual(ouInfos[4].Name, "DIA_Npc_Test_01_04");
            Assert.AreEqual(ouInfos[4].Text, "..");

            Assert.AreEqual(ouInfos[5].Name, "DIA_Npc_Test_01_05");
            Assert.AreEqual(ouInfos[5].Text, "...");
        }

        [TestMethod]
        [DeploymentItem("SampleFiles\\SVM.d")]
        public void OutputUnitsParser_ParseSVM_Test()
        {
            OutputUnitsParser outputUnitsParser = new OutputUnitsParser("SVM.d", OutputUnitsParser.ScriptType.Svm);

            List<OutputUnitInfo> ouInfos = outputUnitsParser.Parse();

            Assert.AreEqual(ouInfos[0].Name, "SVM_1_Test1");
            Assert.AreEqual(ouInfos[0].Text, ".");

            Assert.AreEqual(ouInfos[5].Name, "SVM_1_Test6");
            Assert.AreEqual(ouInfos[5].Text, "......");

            Assert.AreEqual(ouInfos[6].Name, "SVM_2_Test1");
            Assert.AreEqual(ouInfos[6].Text, ".");

            Assert.AreEqual(ouInfos[11].Name, "SVM_2_Test6");
            Assert.AreEqual(ouInfos[11].Text, "......");
        }
    }
}
