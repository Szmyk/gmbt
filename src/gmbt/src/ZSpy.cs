using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        static Thread thread;    

        public static void Run()
        {
            if (Process.GetProcessesByName("zSpy").Length == 0)
            {
                ThreadStart threadStart = () =>
                {
                    Application.Run(new FakeZSpy
                    {
                        Text = "[zSpy]"
                    });
                };

                thread = new Thread(threadStart)
                {
                    IsBackground = true
                };

                thread.Start();
            }
        }

        public static void Abort()
        {
            thread?.Abort();
        }
    }

    class FakeZSpy : Form
    {
        public struct CopyData
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }     

        protected override void SetVisibleCore(bool value)
        {
            CreateHandle();

            base.SetVisibleCore(false);
        }

        public const int WM_COPYDATA = 0x4A;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                var data = (CopyData)m.GetLParam(typeof(CopyData));

                var msg = data.lpData;

                if (msg.StartsWith("Fatal:"))        
                {
                    Logger.Fatal(msg);
                }

                if (Program.Options.CommonTestBuildCompile.ZSpyLevel != ZSpy.Mode.None)
                {
                    if (msg.StartsWith("Warn:"))
                    {
                        Logger.Warn("\t" + msg);
                    }
                    else if (msg.StartsWith("Fault:"))
                    {
                        Logger.Error("\t" + msg);
                    }
                    else
                    {
                        Logger.Minimal("\t" + msg);
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
}
