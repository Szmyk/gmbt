using System;
using System.Collections.Generic;

namespace OutputUnitsUpdater
{
    public class ZenArchiveObject
    {
        public ZenArchiveObjectHeader Header;

        public List<ZenArchiveObjectProperty> Properties = new List<ZenArchiveObjectProperty>();

        public List<ZenArchiveObject> Childs = new List<ZenArchiveObject>();

        public ZenArchiveObject(ZenArchiveObjectHeader header)
        {
            Header = header;
        }

        public ZenArchiveObject(string name, string className, int version)
        {
            Header = new ZenArchiveObjectHeader(name, className, version);
        }

        public void AddProperty(string name, string type, object value)
        {
            Properties.Add(new ZenArchiveObjectProperty(name, type, value));
        }

        public ZenArchiveObject AddChild(string name, string className, int version)
        {
            var zenArchiveObject = new ZenArchiveObject(name, className, version);

            Childs.Add(zenArchiveObject);

            return zenArchiveObject;
        }

        public ZenArchiveObject AddChild(string className)
        {
            var zenArchiveObject = new ZenArchiveObject("%", className, 0);

            Childs.Add(zenArchiveObject);

            return zenArchiveObject;
        }

        public ZenArchiveObject AddChild(ZenArchiveObject zenArchiveObject)
        {
            Childs.Add(zenArchiveObject);

            return zenArchiveObject;
        }
    }

}
