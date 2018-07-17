using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace OutputUnitsUpdater
{
    public class ZenArchiveWriter
    {
        string _path;

        List<ZenArchiveObject> objects = new List<ZenArchiveObject>();

        public ZenArchiveWriter(string path)
        {
            _path = path;
        }

        public ZenArchiveObject AddMainObject(ZenArchiveObject zenArchiveObject)
        {
            objects.Add(zenArchiveObject);

            return zenArchiveObject;
        }

        public ZenArchiveObject AddMainObject(string name, string className, int version)
        {
            var zenArchiveObject = new ZenArchiveObject(name, className, version);

            objects.Add(zenArchiveObject);

            return zenArchiveObject;
        }

        public ZenArchiveObject AddMainObject(string className)
        {
            var zenArchiveObject = new ZenArchiveObject("%", className, 0);

            objects.Add(zenArchiveObject);

            return zenArchiveObject;
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(_path, FileMode.Create), Encoding.Default))
            {
                writeHeader(sw);

                for (int i = 0; i < objects.Count; i++)
                {
                    writeObject(sw, objects[i], i);
                }
            }       
        }

        private int writeObject(StreamWriter sw, ZenArchiveObject obj, int id, string indent = "")
        {
            sw.WriteLine(indent + $"[{obj.Header.Name} {obj.Header.ClassName} {obj.Header.Version} {id}]");

            indent += "\t";

            foreach (var property in obj.Properties)
            {
                sw.WriteLine(indent + property.ToString().TrimEnd());
            }

            foreach (var childObject in obj.Childs)
            {
                id = writeObject(sw, childObject, ++id, indent);
            }

            sw.WriteLine(indent.Remove(indent.Length - 1) + "[]");

            return id;
        }

        private int getAllObjectsCount (List<ZenArchiveObject> objects)
        {
            int count = 0;

            foreach(var obj in objects)
            {
                count += getAllObjectsCount(obj.Childs) + 1;
            }

            return count;
        }

        private void writeHeader (StreamWriter sw)
        {
            sw.WriteLine("ZenGin Archive");
            sw.WriteLine("ver 1");
            sw.WriteLine("zCArchiverGeneric");
            sw.WriteLine("ASCII");
            sw.WriteLine("saveGame 0");
            sw.WriteLine("date " + DateTime.Now.ToString("dd.M.yyyy HH:mm:ss"));
            sw.WriteLine("user " + Environment.UserName);
            sw.WriteLine("END");
            sw.WriteLine("objects " + getAllObjectsCount(objects));
            sw.WriteLine("END");
            sw.WriteLine();
        }
    }
}
