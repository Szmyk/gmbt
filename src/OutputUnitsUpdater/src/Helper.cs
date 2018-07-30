using System.Linq;
using System.Collections.Generic;

namespace OutputUnitsUpdater
{
    public static class OutputUnitsUpdaterHelper
    {
        public static List<OutputUnitInfo> GetDuplicates(List<OutputUnitInfo> list)
        {
            var duplicates = list.Select(x => list.Find(y => x.Name == y.Name && x.Text != y.Text));

            return duplicates.ToList().FindAll(p => list.Contains(p)).GroupBy(x => x.Name).Select(x => x.First()).ToList();           
        }
    }
}
