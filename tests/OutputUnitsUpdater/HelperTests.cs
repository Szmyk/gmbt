using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace OutputUnitsUpdater.Tests
{
    [TestClass]
    public class OutputUnitsUpdaterHelperTests
    {
        [TestMethod]
        public void GetDuplicatesTest()
        {
            var list = new List<OutputUnitInfo>
            {
                new OutputUnitInfo("DIALOG1", "Text1"),
                new OutputUnitInfo("DIALOG1", "Text1"),
                new OutputUnitInfo("DIALOG1", "Text2"),

                new OutputUnitInfo("DIALOG2", "Text3"),
                new OutputUnitInfo("DIALOG2", "Text3"),
                new OutputUnitInfo("DIALOG2", "Text3"),

                new OutputUnitInfo("DIALOG3", "Text4"),
                new OutputUnitInfo("DIALOG3", "Text4"),
                new OutputUnitInfo("DIALOG3", "Text4"),

                new OutputUnitInfo("DIALOG4", "Text5"),
                new OutputUnitInfo("DIALOG4", "Text6"),
                new OutputUnitInfo("DIALOG4", "Text5"),
            };

            var duplicates = OutputUnitsUpdaterHelper.GetDuplicates(list);

            Assert.AreEqual(duplicates.Count, 2);

            Assert.AreEqual(duplicates[0].Name, "DIALOG1");
            Assert.AreEqual(duplicates[1].Name, "DIALOG4");
        }
    }
}
