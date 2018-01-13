using System;
using System.Text;

namespace GMBT
{
    /// <summary>
    /// Wraps the System.Console class, adding enhanced functionality.
    /// </summary>
    internal static class Console
    {      
        public static int BufferWidth
        {
            get => System.Console.BufferWidth;
            set => System.Console.BufferWidth = value;
        }
      
        public static int CursorLeft
        {
            get => System.Console.CursorLeft;
            set => System.Console.CursorLeft = value;
        }

        public static int CursorTop
        {
            get => System.Console.CursorTop;            
            set => System.Console.CursorTop = value;         
        }
      
        public static bool CursorVisible
        {
            get => System.Console.CursorVisible;            
            set => System.Console.CursorVisible = value;            
        }   
        
        public static ConsoleColor ForegroundColor
        {
            get => System.Console.ForegroundColor;
            set => System.Console.ForegroundColor = value;
        }

        public static bool IsOutputRedirected
        {
            get => System.Console.IsOutputRedirected;          
        }
   
        public static Encoding OutputEncoding
        {
            get => System.Console.OutputEncoding;
            set => System.Console.OutputEncoding = value;
        }

        public static int WindowHeight
        {
            get => System.Console.WindowHeight;
            set => System.Console.WindowHeight = value;  
        }

        public static int WindowLeft
        {
            get => System.Console.WindowLeft;           
            set => System.Console.WindowLeft = value;          
        }

        public static int WindowTop
        {
            get => System.Console.WindowTop;           
            set => System.Console.WindowTop = value;           
        }

        public static int WindowWidth
        {
            get => System.Console.WindowWidth;
            set => System.Console.WindowWidth = value;       
        }

        public static void WriteSecondInColor(string first, string second, ConsoleColor color)
        {           
            Write(first);

            var colorBackup = ForegroundColor;

            ForegroundColor = color;

            Write(second);

            ForegroundColor = colorBackup;
        }
    
        public static void Write(object value)
        {
            System.Console.Write(value);
        }
  
        public static void Write(string value)
        {
            System.Console.Write(value);
        }

        public static void Write(string format, object arg0)
        {
            System.Console.Write(format, arg0);
        }
        
        public static void WriteLine()
        {
            System.Console.WriteLine();
        }  

        public static void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey();
        }

        public static void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }
    }
}
