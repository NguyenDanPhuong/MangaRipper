using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MangaRipper.Core.Models
{
    /// <summary>
    /// Configuration for plugins
    /// </summary>
    public class Configuration : IConfiguration
    {
        private string ConfigFile
        {
            get;
        }

        public Configuration(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Cannot find config file.", path);
            }
            ConfigFile = path;
        }

        public IEnumerable<KeyValuePair<string, object>> FindConfigByPrefix(string prefix)
        {
            var json = File.ReadAllText(ConfigFile);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return dict.Where(i => i.Key.StartsWith(prefix));
        }
    }
}
