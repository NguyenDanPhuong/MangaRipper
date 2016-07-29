using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterMangaReader : ChapterBase
    {
        public ChapterMangaReader(string name, string address) : base(name, address) { }

        protected override List<string> ParseImageAddresses(string html)
        {
            var list = new List<string>();
            Regex reg = new Regex(@"<img id=""img"" width=""\d+"" height=""\d+"" src=""(?<Value>[^""]+)""",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), match.Groups["Value"].Value);
                list.Add(value.AbsoluteUri);
            }

            return list;
        }

        protected override List<string> ParsePageAddresses(string html)
        {
            var list = new List<string>();
            list.Add(Address);

            Regex reg = new Regex(@"<option value=""(?<Value>[^""]+)""(| selected=""selected"")>\d+</option>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(new Uri(Address), (match.Groups["Value"].Value )).AbsoluteUri;
                if(list.Contains(value) == false)
                {
                    list.Add(value);
                }                
            }

            return list;
        }
    }
}
