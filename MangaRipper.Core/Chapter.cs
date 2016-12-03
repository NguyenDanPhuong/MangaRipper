using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaRipper.Core;
namespace MangaRipper.Core
{
    [Serializable]
    public class Chapter
    {
        public string Name { get; private set; }
        /// <summary>
        /// Chapter name which safe for using as folder name.
        /// </summary>
        public string NomalizeName => Name.RemoveFileNameInvalidChar();

        public string Link { get; private set; }
        //TODO Move IsBusy into ChapterDownloadTask class
        public bool IsBusy { get; internal set; }
        public Chapter(string name, string link)
        {
            Name = name;
            Link = link;
        }

        public void AddPrefix(int prefix)
        {
            Name = $"[{prefix:000}] - {Name}";
        }
    }
}
