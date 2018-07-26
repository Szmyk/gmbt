using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Szmyk.Utils.StringHelper;
using System.Linq;

namespace OutputUnitsUpdater
{
    public class OutputUnitsUpdater
    {
        string _srcFile;
        string _cslFile;

        List<OutputUnitInfo> _ouList = new List<OutputUnitInfo>();

        public OutputUnitsUpdater (string srcFile, string cslFile)
        {
            _srcFile = srcFile;
            _cslFile = cslFile;
        }

        public void Update()
        {
            parse();

            clearCompiledCutscenes();

            writeCsl();
        }

        public List<OutputUnitInfo> GetDuplicates()
        {
            var duplicates = _ouList.GroupBy(x => x.Name)
                                    .Where(x => x.Count() > 1)
                                    .Select(x => x.Key);

            return _ouList.FindAll(p => duplicates.Contains(p.Name))
                          .GroupBy(x => x.Name)
                          .Select(x => x.First()).ToList();
        }

        List<OutputUnitInfo> parse ()
        {
            var src = new SrcFile(_srcFile);

            foreach (var script in src.GetScripts())
            {
                var output = new OutputUnitsParser(script, getFileType(script)).Parse();

                _ouList.AddRange(output);
            }

            return _ouList;
        }

        private void clearCompiledCutscenes ()
        {
            string ouDirectory = Path.GetDirectoryName(_cslFile);

            if (Directory.Exists(ouDirectory) == false)
            {
                Directory.CreateDirectory(ouDirectory);
            }

            if (File.Exists(ouDirectory + "\\OU.BIN"))
            {
                File.Delete(ouDirectory + "\\OU.BIN");
            }
        }

        void writeCsl ()
        {
            var writer = new ZenArchiveWriter(_cslFile);

            var mainObject = writer.AddMainObject("zCCSLib");
            mainObject.AddProperty("NumOfItems", "int", _ouList.Count);

            for (int i = 0; i < _ouList.Count; i++)
            {
                var block = mainObject.AddChild("zCCSBlock");
                block.AddProperty("blockName", "string", _ouList[i].Name);
                block.AddProperty("numOfBlocks", "int", 1);
                block.AddProperty("subBlock0", "float", 0);

                var atomicBlock = block.AddChild("zCCSAtomicBlock");
                var eventMessage = atomicBlock.AddChild("oCMsgConversation:oCNpcMessage:zCEventMessage");
                eventMessage.AddProperty("subType", "enum", 0);
                eventMessage.AddProperty("text", "string", _ouList[i].Text);
                eventMessage.AddProperty("name", "string", _ouList[i].Name.ToUpper().RemoveSpaces() + ".WAV");   
            }

            writer.Save();
        }

        OutputUnitsParser.ScriptType getFileType(string scriptPath)
        {
            return scriptPath.ToLower().Contains("svm")
                 ? OutputUnitsParser.ScriptType.Svm
                 : OutputUnitsParser.ScriptType.NormalDialogue;
        }
    }
}
