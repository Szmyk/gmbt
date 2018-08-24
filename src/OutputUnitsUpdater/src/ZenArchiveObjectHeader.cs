namespace OutputUnitsUpdater
{
    public class ZenArchiveObjectHeader
    {
        public string Name;
        public string ClassName;
        public int Version;

        public ZenArchiveObjectHeader(string name, string className, int version)
        {
            Name = name;
            ClassName = className;
            Version = version;
        }
    }
}
