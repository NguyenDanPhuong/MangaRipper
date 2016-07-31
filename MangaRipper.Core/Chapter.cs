using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    [Serializable]
    public class Chapter
    {
        public string Name { get; private set; }
        public string Link { get; private set; }
        public bool IsBusy { get; internal set; }
        public Chapter(string name, string link)
        {
            Name = name;
            Link = link;
        }

        public void AddPrefix(int prefix)
        {
            Name = string.Format("[{0:000}] - {1}", prefix, Name);
        }
    }
}
