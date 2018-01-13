namespace OutputUnitsUpdater
{
    public class OutputUnitInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }

        public OutputUnitInfo(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
