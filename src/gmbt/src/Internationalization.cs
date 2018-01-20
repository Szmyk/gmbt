using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GMBT
{
    static class Internationalization
    {
        static Dictionary<string, string> keys;
        static string lang;

        static Dictionary<string, string> readJson(string json)
        {
            return JsonConvert
                   .DeserializeObject<Dictionary<string, string>>(json)
                   .ToDictionary(x => x.Key.Trim(), x => x.Value.Trim());        
        }

        static public void SetLanguage(string code)
        {
            lang = code;
        }

        static public void Init()
        {
            lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            try
            {
                var file = Program.AppData.Languages + lang + ".json";

                if (File.Exists(file) == false)
                {
                    file = Program.AppData.Languages + "en.json";

                    if (File.Exists(file) == false)
                    {
                        throw new FileNotFoundException();
                    }
                }

                var json = File.ReadAllText(Program.AppData.Languages + lang + ".json");

                keys = readJson(json);
            }
            catch
            {
                Program.Logger.Fatal("No language file could be loaded. Reinstall application.");             
            }                  
        }

        static public string Translate(this string key, params object[] arg)
        {
            return string.Format(keys[key], arg);
        }
    }
}
