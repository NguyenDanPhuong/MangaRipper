using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    class ChapterMangaHere : ChapterBase
    {
        public ChapterMangaHere(string name, Uri address) : base(name, address) { }

        protected override List<Uri> ParsePageAddresses(string html)
        {
            var list = new List<Uri>();
            list.Add(Address);
            Regex reg = new Regex(@"<option value=""(?<Value>http://www.mangahere.com/manga/[^""]+)"" (|selected=""selected"")>\d+</option>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }

            return list.Distinct().ToList();
        }

        protected override List<Uri> ParseImageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex("<img src=\"(?<Value>[^\"]+)\" onerror",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }

            return list;
        }
    }
}
