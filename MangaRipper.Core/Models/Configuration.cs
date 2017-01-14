using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MangaRipper.Core.Models
{
    /// <summary>
    /// We have 2 JSON config files.
    /// 1 - application.json
    /// 2 - user.json
    /// 1 is read only and has all settings. Locale in application folder. It makes the app alway have correct config and run good.
    /// 2 is empty and user can override setting in 1 here.
    /// </summary>
    class Configuration
    {
        private string ConfigFile
        {
            get;
        }

        public Configuration()
        {
            ConfigFile = Path.Combine(Environment.CurrentDirectory, "application.json");
        }

        public IEnumerable<KeyValuePair<string, string>> FindConfigByPrefix(string prefix)
        {
            var json = File.ReadAllText(ConfigFile);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dict.Where(i => i.Key.StartsWith(prefix));
        }
    }
}
