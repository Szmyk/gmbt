using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Szmyk.Utils.Directory;
using Szmyk.Utils.StringHelper;

namespace OutputUnitsUpdater
{
    public static class OutputUnitsUpdater
    {
        public static List<OutputUnitInfo> ParseMany(string scriptsPath)
        {
            List<OutputUnitInfo> ouInfos = new List<OutputUnitInfo>();

            DirectoryHelper dialogs = new DirectoryHelper(scriptsPath);

            foreach (string file in dialogs.GetFiles(new[] { "*.d" }))
            {
                OutputUnitsParser outputUnitsParser = new OutputUnitsParser(file, GetDialogFileType(file));

                ouInfos.AddRange(outputUnitsParser.Parse());
            }

            return ouInfos;
        }

        public static void Update (string scriptsPath, string cslFile)
        {
            string ouDirectory = Path.GetDirectoryName(cslFile);

            if (Directory.Exists(ouDirectory) == false)
            {
                Directory.CreateDirectory(ouDirectory);
            }      

            if (File.Exists(ouDirectory + "\\OU.BIN"))
            {
                File.Delete(ouDirectory + "\\OU.BIN");
            }

            List<OutputUnitInfo> ouList = ParseMany(scriptsPath);

            using (StreamWriter sw = new StreamWriter(new FileStream(cslFile, FileMode.OpenOrCreate), Encoding.Default))
            {
                sw.Write(@"ZenGin Archive" + Environment.NewLine +
                    "ver 1" + Environment.NewLine +
                    "zCArchiverGeneric" + Environment.NewLine +
                    "ASCII" + Environment.NewLine +
                    "saveGame 0" + Environment.NewLine +
                    "date " + DateTime.Now.ToString() + Environment.NewLine +
                    "user " + Environment.UserName + Environment.NewLine +
                    "END" + Environment.NewLine +
                    "objects " + (ouList.Count * 3 + 1).ToString() + Environment.NewLine +
                    "END" + Environment.NewLine + Environment.NewLine +

                    "[% zCCSLib 0 0]" + Environment.NewLine +
                    "\t" + "NumOfItems=int:" + ouList.Count + Environment.NewLine);

                for (int i = 0; i < ouList.Count; i++)
                {
                    int j = i * 3;
                   
                    sw.Write(@"" +
                        "\t" + "[% zCCSBlock 0 " + (j + 1).ToString() + "]" + Environment.NewLine +
                        "\t\t" + "blockName=string:" + ouList[i].Name + Environment.NewLine +
                        "\t\t" + "numOfBlocks=int:1" + Environment.NewLine +
                        "\t\t" + "subBlock0=float:0" + Environment.NewLine +
                        "\t\t" + "[% zCCSAtomicBlock 0 " + (j + 2).ToString() + "]" + Environment.NewLine +
                        "\t\t\t" + "[% oCMsgConversation:oCNpcMessage:zCEventMessage 0 " + (j + 3).ToString() + "]" + Environment.NewLine +
                        "\t\t\t\t" + "subType=enum:0" + Environment.NewLine +
                        "\t\t\t\t" + "text=string:" + ouList[i].Text + Environment.NewLine +
                        "\t\t\t\t" + "name=string:" + ouList[i].Name.ToUpper().RemoveSpaces() + ".WAV" + Environment.NewLine +
                        "\t\t\t" + "[]" + Environment.NewLine +
                        "\t\t" + "[]" + Environment.NewLine +
                        "\t" + "[]" + Environment.NewLine);
                }

                sw.WriteLine("[]");
            }
        }

        public static OutputUnitsParser.ScriptType GetDialogFileType(string scriptPath)
        {
            return scriptPath.ToLower().Contains("svm")
                 ? OutputUnitsParser.ScriptType.Svm
                 : OutputUnitsParser.ScriptType.NormalDialogue;
        }
    }
}
