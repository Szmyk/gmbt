using System;
using System.Collections.Generic;

namespace OutputUnitsUpdater
{
    public struct ZenArchiveObjectProperty
    {
        public string Name;
        public string Type;
        public object Value;

        public ZenArchiveObjectProperty(string name, string type, object value)
        {
            Name = name;

            Type = type;

            Value = value;         
        }

        public override string ToString()
        {
            return string.Format($"{Name}={Type}:{Value}");
        }
    }
}
