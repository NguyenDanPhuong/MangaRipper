using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class SiteInformation
    {
        public string Name { get; private set; }
        public string Link { get; private set; }
        public string Language { get; private set; }

        public SiteInformation(string name, string link, string language)
        {
            Name = name;
            Link = link;
            Language = language;
        }
    }
}
