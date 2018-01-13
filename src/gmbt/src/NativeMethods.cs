using System.Runtime.InteropServices;

namespace GMBT
{
    /// <summary> 
    /// Performs imports of Windows native methods.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("Imagehlp.dll", EntryPoint = "MapFileAndCheckSum")]
        static public extern uint MapFileAndCheckSum(string Filename, out uint HeaderSum, out uint CheckSum);
    }
}
