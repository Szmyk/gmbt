//   _____       _   _     _        __  __           _   ____        _ _     _   _______          _ 
//  / ____|     | | | |   (_)      |  \/  |         | | |  _ \      (_) |   | | |__   __|        | |
// | |  __  ___ | |_| |__  _  ___  | \  / | ___   __| | | |_) |_   _ _| | __| |    | | ___   ___ | |
// | | |_ |/ _ \| __| '_ \| |/ __| | |\/| |/ _ \ / _` | |  _ <| | | | | |/ _` |    | |/ _ \ / _ \| |
// | |__| | (_) | |_| | | | | (__  | |  | | (_) | (_| | | |_) | |_| | | | (_| |    | | (_) | (_) | |
//  \_____|\___/ \__|_| |_|_|\___| |_|  |_|\___/ \__,_| |____/ \__,_|_|_|\__,_|    |_|\___/ \___/|_|                                                                                                                                                                                                 
//
//  Gothic Mod Build Tool — simple tool designed to help in testing and building Gothic and Gothic 2 Night of the Raven mods.
//
//  Copyright(c) 2017 Szymon 'Szmyk' Zak
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;

using I18NPortable;

namespace GMBT
{
    /// <summary>
    /// Performs watching of currently compiling assets by game.
    /// </summary>
    internal class CompilingAssetsWatcher : FileSystemWatcher
    {
        public Action<string> OnFileCompile { get; set; }
      
        public CompilingAssetsWatcher(string path, string filter) : base(path, filter)
        {
            NotifyFilter = NotifyFilters.LastWrite;

            IncludeSubdirectories = true;

            Changed += OnChanged;
        }

        void OnChanged(object source, FileSystemEventArgs e)
        {
            if (File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            if (Program.Options.Common.ShowCompilingAssets)
            {
                Console.WriteLine("\t" + "Compiled".Translate() + ": " + e.Name);
            }

            Program.Logger.Trace("\t" + "Compiled".Translate() + ": " + e.Name);

            OnFileCompile?.Invoke(e.FullPath.ToUpper());
        }

        public void Start() => EnableRaisingEvents = true;

        public void Stop() => EnableRaisingEvents = false;
    }
}
