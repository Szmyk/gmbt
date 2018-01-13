using System.Diagnostics;

namespace GMBT
{
    /// <summary> 
    /// Performs running the zSpy logger.
    /// </summary>
    internal static class ZSpy
    {
        public enum Mode
        {
            None = 0,
            Low = 4,
            Medium = 5,
            High = 10
        }

        public static void Run()
        {
            Process.Start(Program.AppData.Tools + "zSpy.exe");
        }
    }
}
