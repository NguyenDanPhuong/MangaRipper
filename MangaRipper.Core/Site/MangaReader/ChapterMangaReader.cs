using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper.Core
{
    [Serializable]
    public class ChapterMangaReader : ChapterBase
    {
        public ChapterMangaReader(string name, Uri address) : base(name, address) { }

        protected override List<Uri> ParseImageAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"<img id=""img"" width=""\d+"" height=""\d+"" src=""(?<Value>[^""]+)""",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                list.Add(value);
            }

            return list;
        }

        protected override List<Uri> ParsePageAddresses(string html)
        {
            var list = new List<Uri>();
            list.Add(Address);

            Regex reg = new Regex(@"<option value=""(?<Value>[^""]+)""(| selected=""selected"")>\d+</option>",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, (match.Groups["Value"].Value ));
                if(list.Contains(value) == false)
                {
                    list.Add(value);
                }                
            }

            return list;
        }
    }
}
