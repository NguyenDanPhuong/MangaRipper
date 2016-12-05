using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaRipper.Core;
namespace MangaRipper.Core
{
    public class Chapter
    {
        public string Name { get; private set; }
        /// <summary>
        /// Chapter name which safe for using as folder name.
        /// </summary>
        public string NomalizeName => Name.RemoveFileNameInvalidChar();

        public string Url { get; private set; }

        public Chapter(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public void AddPrefix(int prefix)
        {
            Name = $"[{prefix:000}] - {Name}";
        }
    }
}
