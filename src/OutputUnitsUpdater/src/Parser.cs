using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OutputUnitsUpdater
{
    public class OutputUnitsParser
    {
        private readonly Regex dialogPattern = new Regex(@"^\s*AI_Output[\w]*\s*\(\s*(.*)\s*,\s*\w*\s*,\s*""(.*)""\s*\)\s*;\s*//(.*)", RegexOptions.IgnoreCase);
        private readonly Regex svmPattern = new Regex(@"^\s*(.*)\s*=\s*""(.*)""\s*;\s*//(.*)", RegexOptions.IgnoreCase);

        public enum ScriptType
        {
            NormalDialogue,
            Svm
        }

        readonly string file;
        readonly ScriptType scriptType;

        public OutputUnitsParser (string file, ScriptType scriptType)
        {
            this.file = file;
            this.scriptType = scriptType;
        }

        public List<OutputUnitInfo> Parse()
        {
            List<OutputUnitInfo> ouList = new List<OutputUnitInfo>();

            var pattern = scriptType == ScriptType.NormalDialogue
                        ? dialogPattern
                        : svmPattern;

            foreach (string line in File.ReadAllLines(file, Encoding.Default).ToList())
            {
                if (pattern.IsMatch(line))
                {
                    string[] splitted = pattern.Split(line);

                    ouList.Add(new OutputUnitInfo(splitted[2], splitted[3]));
                }
            }

            return ouList;
        }
    }
}
