using System;

namespace OutputUnitsUpdater
{
    public class MatchingFilesNotFoundException : Exception
    {
        public readonly string Line;
        public readonly int LineNumber;

        public MatchingFilesNotFoundException(string line, int lineNumber)
        {
            Line = line;
            LineNumber = lineNumber;
        }
    }
}
