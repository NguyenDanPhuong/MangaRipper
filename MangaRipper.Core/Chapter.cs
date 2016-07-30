using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Chapter
    {
        public string Name { get; private set; }
        public string Link { get; private set; }
        public Chapter(string name, string link)
        {
            Name = name;
            Link = link;
        }
    }
}
