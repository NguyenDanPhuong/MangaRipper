using System.Collections.Generic;

namespace MangaRipper.Core.Models
{
    public interface IConfiguration
    {
        IEnumerable<KeyValuePair<string, object>> FindConfigByPrefix(string prefix);
    }
}