using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

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

            var content = File.ReadAllText(file, Encoding.Default);

            content = stripComments(content);

            foreach (string line in content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                if (pattern.IsMatch(line))
                {
                    string[] splitted = pattern.Split(line);

                    var text = splitted[3].Trim();

                    if (text != string.Empty)
                    {
                        ouList.Add(new OutputUnitInfo(splitted[2], text));
                    }
                }
            }

            return ouList;
        }

        string stripComments (string code)
        {
            StringBuilder sb = new StringBuilder(code.Length + 1);

            bool lineStart = false;
            int startLineComment = -1;
            int startBlockComment = -1;
            int lastOffset = 0;

            for (int i = 1; i < code.Length; i += 2)
            {
                if (code[i] == '\n'
                || code[i] == '\r')
                {
                    lineStart = true;

                    if (startLineComment > 0)
                    {
                        startLineComment = -1;
                        lastOffset = i;
                    }

                    continue;
                }

                if (lineStart)
                {
                    if (( startBlockComment == -1 )
                    && ( startLineComment == -1 )
                    && ( code[i] == '/' ))
                    {
                        if (code[i + 1] == '/')
                        {
                            startLineComment = i - 1;

                            sb.Append(code.Substring(lastOffset, startLineComment - lastOffset));
                        }
                        else if (code[i - 1] == '/')
                        {
                            startLineComment = i - 2;

                            sb.Append(code.Substring(lastOffset, startLineComment - lastOffset));
                        }
                    }

                    if (char.IsWhiteSpace(code[i]) == false)
                    {
                        lineStart = false;
                    }
                }

                if (lineStart == false)
                {
                    if (( startBlockComment == -1 )
                    && ( startLineComment == -1 ))
                    {
                        if (code[i] == '/'
                        && code[i + 1] == '*')
                        {
                            startBlockComment = i;

                            sb.Append(code.Substring(lastOffset, startBlockComment - lastOffset));
                        }
                        else if (code[i - 1] == '/'
                             && code[i] == '*')
                        {
                            startBlockComment = i - 1;

                            sb.Append(code.Substring(lastOffset, startBlockComment - lastOffset));
                        }
                    }
                    else if (startBlockComment > -1)
                    {
                        if (code[i] == '*'
                        && code[i + 1] == '/')
                        {
                            lastOffset = i + 1;

                            startBlockComment = -1;
                        }
                        else if (code[i - 1] == '*'
                             && code[i] == '/')
                        {
                            lastOffset = i;

                            startBlockComment = -1;
                        }
                    }
                }
            }

            sb.Append(code.Substring(lastOffset, code.Length - lastOffset));

            return sb.ToString();
        }
    }
}
