using System;
using System.Collections.Generic;

namespace OutputUnitsUpdater
{
    public class ZenArchiveObjectProperty
    {
        Dictionary<Type, string> typesMap = new Dictionary<Type, string>()
        {
            { typeof(string), "string" },
            { typeof(Int32), "int" },
            { typeof(float), "float" },
            { typeof(Enum), "enum" }
        };

        public string Name;
        public string Type;
        public object Value;

        public ZenArchiveObjectProperty(string name, object value)
        {
            Name = name;

            var valueType = value.GetType();

            if (valueType.IsEnum)
            {
                Type = typesMap[typeof(Enum)];
            }
            else
            {
                Type = typesMap[value.GetType()];
            }

            if (Type == "enum")
            {
                Value = (int)Enum.ToObject(value.GetType(), value);
            }
            else
            {
                Value = value;
            }
        }

        public override string ToString()
        {
            return string.Format($"{Name}={Type}:{Value}");
        }
    }
}
