using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace GMBT
{
    /// <summary> 
    /// Implements console line interface progress bar.
    /// </summary>
    public class ProgressBar : IDisposable
    {
        private const int blockCount = 20;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(0.1);

        private readonly List<string> animation = new List<string>
        {
            "-",
            "\\",
            "|",
            "/"
        };

        private int progress;
        private readonly int total;

        private readonly Timer timer;

        private string currentText = string.Empty;
        private bool disposed;
        private int animationIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar(string text, int total)
        {
            Logger.WriteLineToFile(text);

            if (Logger.Verbosity >= VerbosityLevel.Detailed)
            {
                Console.WriteLine(text);
            }
            else if (Logger.Verbosity == VerbosityLevel.Normal)
            {
                Console.Write(text + " ");
            }
            else if (Logger.Verbosity != VerbosityLevel.Quiet)
            {
                Console.Write(text);
            }

            this.total = total;

            timer = new Timer(timerHandler);
        
            if (!Console.IsOutputRedirected)
            {
                resetTimer();
            }
        }

        /// <summary>
        /// Sets progress to given number.
        /// </summary>
        /// <param name="number"></param>
        public void SetProgress(int number)
        {
            progress = number;
        }

        /// <summary>
        /// Increases progress by one.
        /// </summary>
        public void Increase()
        {
            progress += 1;
        }

        private int getPercentage() => ( progress * 100 ) / total;

        private void timerHandler(object state)
        {
            lock (timer)
            {
                if (disposed)
                {
                    return;
                }
                
                int progressBlockCount = Convert.ToInt16(blockCount * Convert.ToDouble(progress) / total);

                string text = string.Format("[{0}{1}] {2,3}% {3}",
                    new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                    getPercentage(),
                    animation[animationIndex++ % 3]);

                update(text);

                resetTimer();
            }
        }

        private int getCommonTextLenght (string text)
        {
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);

            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            return commonPrefixLength;
        }

        private void deleteOverlappingChars(string text, StringBuilder outputBuilder)
        {
            int overlapCount = currentText.Length - text.Length;
      
            outputBuilder.Append(' ', overlapCount);
            outputBuilder.Append('\b', overlapCount);
        }

        private void update(string text)
        {
            int commonPrefixLength = getCommonTextLenght(text);

            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            outputBuilder.Append(text.Substring(commonPrefixLength));

            if (currentText.Length - text.Length > 0)
            {
                deleteOverlappingChars(text, outputBuilder);
            }

            if (Logger.Verbosity == VerbosityLevel.Normal)
            {
                Console.Write(outputBuilder);
            }        

            currentText = text;
        }

        private void resetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ProgressBar"/> object.
        /// </summary>
        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                update(string.Empty);          
            }

            if (Logger.Verbosity >= VerbosityLevel.Normal)
            {
                Console.WriteLine("Done".Translate(), ConsoleColor.Green);
            }
            else if (Logger.Verbosity == VerbosityLevel.Minimal)
            {
                Console.WriteLine();
            }
        }
    }
}
