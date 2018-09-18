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

        static Dictionary<string, string> readJson(string json)
        {
            return JsonConvert
                   .DeserializeObject<Dictionary<string, string>>(json)
                   .ToDictionary(x => x.Key.Trim(), x => x.Value.Trim());        
        }

        static public void SetLanguage(string code)
        {
            var lang = code;

            Init(lang);
        }

        static public void Init()
        {
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();

            if (lang == "en" || lang == "pl" || lang == "sk")
            {
                Init(lang);
            }
            else
            {
                Init("en");
            }          
        }

        static public void Init(string lang)
        {
            lang = lang.ToLower();

            try
            {
                var file = Path.Combine(Program.AppData.Languages, lang + ".json");

                if (File.Exists(file) == false)
                {
                    Logger.Fatal($"Not found language file ({file}).");
                }

                var json = File.ReadAllText(file);

                keys = readJson(json);
            }
            catch
            {
                Logger.Fatal("No language file could be loaded. Please reinstall application.");             
            }                  
        }

        static public string Translate(this string key, params object[] arg)
        {
            return string.Format(keys[key], arg);
        }
    }
}
