using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    class ChapterMangaHere : ChapterBase
    {
        public ChapterMangaHere(string name, string address) : base(name, address) { }

        protected override List<string> ParsePageAddresses(string html)
        {
            var list = new List<string>();
            list.Add(Address);
            Regex reg = new Regex(@"<option value=""(?<Value>http://www.mangahere.com/manga/[^""]+)"" (|selected=""selected"")>\d+</option>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), match.Groups["Value"].Value).AbsoluteUri;
                list.Add(value);
            }

            return list.Distinct().ToList();
        }

        protected override List<string> ParseImageAddresses(string html)
        {
            var list = new List<string>();
            Regex reg = new Regex("<img src=\"(?<Value>[^\"]+)\" onerror",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), match.Groups["Value"].Value).AbsoluteUri;
                list.Add(value);
            }

            return list;
        }
    }
}
